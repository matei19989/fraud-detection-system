using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Application.Services;

public class FraudDetectionService : IFraudDetectionService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRuleEvaluationEngine _ruleEngine;
    private readonly ILogger<FraudDetectionService> _logger;
    private readonly IMemoryCache _cache;
    
    private const string ActiveRulesCacheKey = "ActiveFraudRules";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public FraudDetectionService(
        IApplicationDbContext dbContext,
        IRuleEvaluationEngine ruleEngine,
        ILogger<FraudDetectionService> logger,
        IMemoryCache cache)
    {
        _dbContext = dbContext;
        _ruleEngine = ruleEngine;
        _logger = logger;
        _cache = cache;
    }

    public async Task<FraudAnalysisResult> AnalyzeTransactionAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Starting fraud analysis for transaction {TransactionId}, Account: {AccountId}, Amount: {Amount}",
            transaction.Id,
            transaction.AccountId,
            transaction.Amount);

        try
        {
            var account = await _dbContext.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountId == transaction.AccountId, cancellationToken);

            if (account == null)
            {
                _logger.LogWarning(
                    "Account {AccountId} doesn't exist for transaction {TransactionId}",
                    transaction.AccountId,
                    transaction.Id);

                return CreateSafeResult(transaction);
            }

            // Get rules from cache or database
            var activeRules = await GetActiveRulesAsync(cancellationToken);

            _logger.LogInformation(
                "Evaluating {RuleCount} active fraud rules for transaction {TransactionId} (cached: {IsCached})",
                activeRules.Count,
                transaction.Id,
                _cache.TryGetValue(ActiveRulesCacheKey, out _));

            // PARALLEL EVALUATION - All rules run simultaneously
            var evaluationTasks = activeRules.Select(rule =>
                _ruleEngine.EvaluateRuleAsync(rule, transaction, account, cancellationToken));

            var ruleResults = await Task.WhenAll(evaluationTasks);

            _logger.LogInformation(
                "Completed parallel evaluation of {RuleCount} rules in transaction {TransactionId}",
                activeRules.Count,
                transaction.Id);

            // Process triggered rules
            var alertsCreated = new List<FraudAlert>();
            
            foreach (var result in ruleResults.Where(r => r.Triggered))
            {
                var rule = activeRules.First(r => r.Id == result.RuleId);
                
                _logger.LogWarning(
                    "Rule {RuleName} triggered for transaction {TransactionId} with score {Score}",
                    rule.Name,
                    transaction.Id,
                    result.Score);

                rule.RecordTrigger();

                var alert = new FraudAlert(
                    transaction.Id,
                    rule.Name,
                    rule.RiskLevel,
                    result.Score,
                    $"Rule '{rule.Name}' triggered",
                    result.Details,
                    rule.Id);

                alertsCreated.Add(alert);
                await _dbContext.FraudAlerts.AddAsync(alert, cancellationToken);
                transaction.AddFraudAlert(alert);
            }

            var totalScore = CalculateFraudScore(ruleResults.ToList(), activeRules);
            var riskLevel = DetermineRiskLevel(totalScore);

            transaction.UpdateFraudScore(totalScore, riskLevel);

            if (riskLevel >= FraudRiskLevel.High)
            {
                _logger.LogWarning(
                    "HIGH RISK TRANSACTION detected - TransactionId: {TransactionId}, Score: {Score}, Risk: {Risk}",
                    transaction.Id,
                    totalScore,
                    riskLevel);

                transaction.MarkForReview();
            }
            else if (riskLevel == FraudRiskLevel.Medium)
            {
                _logger.LogInformation(
                    "Medium risk transaction detected - TransactionId: {TransactionId}, Score: {Score}",
                    transaction.Id,
                    totalScore);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Fraud analysis completed for transaction {TransactionId} - Score: {Score}, Risk: {Risk}, Alerts: {AlertCount}",
                transaction.Id,
                totalScore,
                riskLevel,
                alertsCreated.Count);

            return new FraudAnalysisResult
            {
                FraudScore = totalScore,
                RiskLevel = riskLevel.ToString(),
                AlertsCreated = alertsCreated,
                RuleResults = ruleResults.ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error during fraud analysis for transaction {TransactionId}",
                transaction.Id);

            transaction.MarkForReview();
            await _dbContext.SaveChangesAsync(cancellationToken);

            throw;
        }
    }

    private async Task<List<FraudRule>> GetActiveRulesAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            ActiveRulesCacheKey,
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                entry.SetPriority(CacheItemPriority.High);
                
                _logger.LogInformation("Loading active fraud rules from database (cache miss)");
                
                return await _dbContext.FraudRules
                    .Where(r => r.IsActive)
                    .OrderByDescending(r => r.Priority)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }) ?? new List<FraudRule>();
    }

    private double CalculateFraudScore(
        List<RuleEvaluationResult> ruleResults,
        List<FraudRule> allRules)
    {
        if (!ruleResults.Any(r => r.Triggered))
        {
            return 0;
        }

        var triggeredResults = ruleResults.Where(r => r.Triggered).ToList();

        if (triggeredResults.Count == 0)
        {
            return 0;
        }

        var totalScore = triggeredResults.Sum(r => r.Score);

        var weightedScore = triggeredResults
            .Join(allRules,
                result => result.RuleId,
                rule => rule.Id,
                (result, rule) => result.Score * (rule.Priority / 10.0))
            .Sum();

        return Math.Min(100, Math.Max(totalScore, weightedScore));
    }

    private FraudRiskLevel DetermineRiskLevel(double fraudScore)
    {
        return fraudScore switch
        {
            >= 90 => FraudRiskLevel.Critical,
            >= 75 => FraudRiskLevel.High,
            >= 50 => FraudRiskLevel.Medium,
            >= 25 => FraudRiskLevel.Low,
            _ => FraudRiskLevel.None
        };
    }

    private FraudAnalysisResult CreateSafeResult(Transaction transaction)
    {
        transaction.UpdateFraudScore(20, FraudRiskLevel.Low);

        return new FraudAnalysisResult
        {
            FraudScore = 20,
            RiskLevel = FraudRiskLevel.Low.ToString(),
            AlertsCreated = new List<FraudAlert>(),
            RuleResults = new List<RuleEvaluationResult>()
        };
    }
}