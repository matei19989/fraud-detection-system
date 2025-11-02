using System.Text.Json;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FraudDetection.Application.DTOs.RuleConditions;

namespace FraudDetection.Application.Services.RuleEngine;

public class RuleEvaluationEngine : IRuleEvaluationEngine
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<RuleEvaluationEngine> _logger;

    public RuleEvaluationEngine(
        IApplicationDbContext dbContext,
        ILogger<RuleEvaluationEngine> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<RuleEvaluationResult> EvaluateRuleAsync(
        FraudRule rule,
        Transaction transaction,
        Account account,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Evaluating rule {RuleName} for transaction {TransactionId}",
                rule.Name,
                transaction.Id);

            // Parse the conditions JSON
            var condition = JsonSerializer.Deserialize<RuleCondition>(
                rule.ConditionsJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new RuleConditionConverter() }
                });

            if (condition == null)
            {
                _logger.LogWarning("Failed to parse conditions for rule {RuleId}", rule.Id);
                return CreateResult(rule, false, 0, "Failed to parse rule conditions");
            }

            // Evaluate the condition
            var (triggered, score, details) = await EvaluateConditionAsync(
                condition,
                transaction,
                account,
                cancellationToken);

            return CreateResult(rule, triggered, score, details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating rule {RuleId}", rule.Id);
            return CreateResult(rule, false, 0, $"Error: {ex.Message}");
        }
    }

    private async Task<(bool Triggered, double Score, string Details)> EvaluateConditionAsync(
        RuleCondition condition,
        Transaction transaction,
        Account account,
        CancellationToken cancellationToken)
    {
        return condition switch
        {
            AmountThresholdCondition amountCondition =>
                EvaluateAmountThreshold(amountCondition, transaction),

            VelocityCondition velocityCondition =>
                await EvaluateVelocityAsync(velocityCondition, transaction, cancellationToken),

            LocationAnomalyCondition locationCondition =>
                await EvaluateLocationAnomalyAsync(locationCondition, transaction, account, cancellationToken),

            NewAccountCondition newAccountCondition =>
                EvaluateNewAccount(newAccountCondition, transaction, account),

            UnusualMerchantCondition merchantCondition =>
                EvaluateUnusualMerchant(merchantCondition, transaction),

            TimeOfDayCondition timeCondition =>
                EvaluateTimeOfDay(timeCondition, transaction),

            AmountDeviationCondition deviationCondition =>
                await EvaluateAmountDeviationAsync(deviationCondition, transaction, account, cancellationToken),

            CompositeCondition compositeCondition =>
                await EvaluateCompositeAsync(compositeCondition, transaction, account, cancellationToken),

            _ => (false, 0, "Unknown condition type")
        };
    }

    private (bool, double, string) EvaluateAmountThreshold(
        AmountThresholdCondition condition,
        Transaction transaction)
    {
        var amount = transaction.Amount.Amount;
        var triggered = condition.Operator.ToLower() switch
        {
            "greaterthan" => amount > condition.Threshold,
            "lessthan" => amount < condition.Threshold,
            "equals" => amount == condition.Threshold,
            _ => false
        };

        var score = triggered ? 30 : 0;
        var details = triggered
            ? $"Amount {amount:C} {condition.Operator} threshold {condition.Threshold:C}"
            : "Amount within normal range";

        return (triggered, score, details);
    }

    private async Task<(bool, double, string)> EvaluateVelocityAsync(
        VelocityCondition condition,
        Transaction transaction,
        CancellationToken cancellationToken)
    {
        var timeWindow = DateTime.UtcNow.AddMinutes(-condition.TimeWindowMinutes);

        var recentTransactionCount = await _dbContext.Transactions
            .CountAsync(
                t => t.AccountId == transaction.AccountId &&
                     t.TransactionDate >= timeWindow &&
                     t.Id != transaction.Id,
                cancellationToken);

        var triggered = recentTransactionCount >= condition.TransactionCount;
        var score = triggered ? Math.Min(50, recentTransactionCount * 10) : 0;
        var details = triggered
            ? $"{recentTransactionCount} transactions in {condition.TimeWindowMinutes} minutes (threshold: {condition.TransactionCount})"
            : $"Normal velocity: {recentTransactionCount} transactions";

        return (triggered, score, details);
    }

    private async Task<(bool, double, string)> EvaluateLocationAnomalyAsync(
        LocationAnomalyCondition condition,
        Transaction transaction,
        Account account,
        CancellationToken cancellationToken)
    {
        if (account.LastKnownLocation == null)
        {
            return (false, 0, "No previous location to compare");
        }

        var timeWindow = DateTime.UtcNow.AddMinutes(-condition.TimeWindowMinutes);

        var lastTransaction = await _dbContext.Transactions
            .Where(t => t.AccountId == transaction.AccountId &&
                       t.TransactionDate >= timeWindow &&
                       t.Id != transaction.Id)
            .OrderByDescending(t => t.TransactionDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastTransaction == null)
        {
            return (false, 0, "No recent transaction to compare");
        }

        var distance = transaction.Location.DistanceInKilometersTo(lastTransaction.Location);
        var triggered = distance > condition.MaxDistanceKm;
        var score = triggered ? Math.Min(60, distance / 100 * 10) : 0;
        var details = triggered
            ? $"Impossible travel: {distance:F2}km in {condition.TimeWindowMinutes} minutes"
            : $"Normal location: {distance:F2}km from last transaction";

        return (triggered, score, details);
    }

    private (bool, double, string) EvaluateNewAccount(
        NewAccountCondition condition,
        Transaction transaction,
        Account account)
    {
        var accountAge = (DateTime.UtcNow - account.RegistrationDate).TotalDays;
        var isNewAccount = accountAge <= condition.AccountAgeDays;

        if (!isNewAccount)
        {
            return (false, 0, $"Account age: {accountAge:F1} days (not new)");
        }

        var amountCheck = condition.MaxTransactionAmount.HasValue
            ? transaction.Amount.Amount > condition.MaxTransactionAmount.Value
            : false;

        var triggered = isNewAccount && (condition.MaxTransactionAmount == null || amountCheck);
        var score = triggered ? 40 : 0;
        var details = triggered
            ? $"New account ({accountAge:F1} days) with high-value transaction"
            : $"New account ({accountAge:F1} days) with normal transaction";

        return (triggered, score, details);
    }

    private (bool, double, string) EvaluateUnusualMerchant(
        UnusualMerchantCondition condition,
        Transaction transaction)
    {
        var triggered = condition.HighRiskCategories.Contains(
            transaction.Merchant.Category,
            StringComparer.OrdinalIgnoreCase);

        var score = triggered ? 25 : 0;
        var details = triggered
            ? $"High-risk merchant category: {transaction.Merchant.Category}"
            : $"Normal merchant category: {transaction.Merchant.Category}";

        return (triggered, score, details);
    }

    private (bool, double, string) EvaluateTimeOfDay(
        TimeOfDayCondition condition,
        Transaction transaction)
    {
        var hour = transaction.TransactionDate.Hour;
        var triggered = hour >= condition.StartHour && hour <= condition.EndHour;
        var score = triggered ? 15 : 0;
        var details = triggered
            ? $"Transaction at unusual hour: {hour}:00"
            : $"Transaction at normal hour: {hour}:00";

        return (triggered, score, details);
    }

    private async Task<(bool, double, string)> EvaluateAmountDeviationAsync(
        AmountDeviationCondition condition,
        Transaction transaction,
        Account account,
        CancellationToken cancellationToken)
    {
        if (account.TotalTransactions < condition.MinimumTransactionCount)
        {
            return (false, 0, "Insufficient transaction history");
        }

        var average = account.AverageTransactionAmount;
        var deviation = Math.Abs(transaction.Amount.Amount - average);
        var threshold = average * (decimal)condition.StandardDeviationMultiplier;

        var triggered = deviation > threshold;
        var score = triggered ? Math.Min(40, (double)(deviation / average) * 10) : 0;
        var details = triggered
            ? $"Amount {transaction.Amount.Amount:C} deviates significantly from average {average:C}"
            : $"Amount {transaction.Amount.Amount:C} within normal range (avg: {average:C})";

        return (triggered, score, details);
    }

    private async Task<(bool, double, string)> EvaluateCompositeAsync(
        CompositeCondition condition,
        Transaction transaction,
        Account account,
        CancellationToken cancellationToken)
    {
        var results = new List<(bool Triggered, double Score, string Details)>();

        foreach (var subCondition in condition.Conditions)
        {
            var result = await EvaluateConditionAsync(
                subCondition,
                transaction,
                account,
                cancellationToken);
            results.Add(result);
        }

        bool triggered;
        if (condition.Logic.Equals("OR", StringComparison.OrdinalIgnoreCase))
        {
            triggered = results.Any(r => r.Triggered);
        }
        else // AND
        {
            triggered = results.All(r => r.Triggered);
        }

        var score = results.Where(r => r.Triggered).Sum(r => r.Score);
        var details = string.Join("; ", results.Select(r => r.Details));

        return (triggered, score, details);
    }

    private RuleEvaluationResult CreateResult(
        FraudRule rule,
        bool triggered,
        double score,
        string details)
    {
        return new RuleEvaluationResult
        {
            RuleId = rule.Id,
            RuleName = rule.Name,
            Triggered = triggered,
            Score = score,
            Details = details
        };
    }
}