using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.QueryHandlers;

public class GetDashboardStatisticsQueryHandler : IRequestHandler<GetDashboardStatisticsQuery, DashboardStatisticsDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetDashboardStatisticsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardStatisticsDto> Handle(GetDashboardStatisticsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var weekAgo = today.AddDays(-7);
        var monthAgo = today.AddDays(-30);

        // Execute in parallel 
        var statisticsTask = GetTransactionStatistics(today, weekAgo, monthAgo, cancellationToken);
        var alertsTask = GetAlertStatistics(today, cancellationToken);
        var amountsTask = GetAmountStatistics(today, cancellationToken);
        var riskBreakdownTask = GetRiskBreakdown(today, cancellationToken);

        await Task.WhenAll(statisticsTask, alertsTask, amountsTask, riskBreakdownTask);

        var (todayCount, weekCount, monthCount) = await statisticsTask;
        var (activeAlerts, resolvedToday) = await alertsTask;
        var (totalProcessed, totalFlagged) = await amountsTask;
        var (highRisk, mediumRisk, lowRisk) = await riskBreakdownTask;

        var fraudDetectionRate = todayCount > 0
            ? (decimal)highRisk / todayCount * 100
            : 0;

        return new DashboardStatisticsDto
        {
            TotalTransactionsToday = todayCount,
            TotalTransactionsThisWeek = weekCount,
            TotalTransactionsThisMonth = monthCount,
            ActiveAlertsCount = activeAlerts,
            ResolvedAlertsToday = resolvedToday,
            FraudDetectionRate = Math.Round(fraudDetectionRate, 2),
            TotalAmountProcessedToday = totalProcessed,
            TotalAmountFlaggedToday = totalFlagged,
            HighRiskTransactionsToday = highRisk,
            MediumRiskTransactionsToday = mediumRisk,
            LowRiskTransactionsToday = lowRisk
        };
    }

    private async Task<(int Today, int Week, int Month)> GetTransactionStatistics(
        DateTime today,
        DateTime weekAgo,
        DateTime monthAgo,
        CancellationToken cancellationToken)
    {
        // SQL aggregate counts
        var todayCount = await _dbContext.Transactions
            .CountAsync(t => t.CreatedAt >= today, cancellationToken);

        var weekCount = await _dbContext.Transactions
            .CountAsync(t => t.CreatedAt >= weekAgo, cancellationToken);

        var monthCount = await _dbContext.Transactions
            .CountAsync(t => t.CreatedAt >= monthAgo, cancellationToken);

        return (todayCount, weekCount, monthCount);
    }

    private async Task<(int Active, int ResolvedToday)> GetAlertStatistics(
        DateTime today,
        CancellationToken cancellationToken)
    {
        var activeAlerts = await _dbContext.FraudAlerts
            .CountAsync(
                a => a.Status == AlertStatus.New || a.Status == AlertStatus.Investigating,
                cancellationToken);

        var resolvedToday = await _dbContext.FraudAlerts
            .CountAsync(
                a => a.Status == AlertStatus.Resolved && a.UpdatedAt >= today,
                cancellationToken);

        return (activeAlerts, resolvedToday);
    }

    private async Task<(decimal TotalProcessed, decimal TotalFlagged)> GetAmountStatistics(
        DateTime today,
        CancellationToken cancellationToken)
    {
        var amounts = await _dbContext.Transactions
            .Where(t => t.CreatedAt >= today)
            .Select(t => new
            {
                Amount = t.Amount.Amount,
                IsHighRisk = t.RiskLevel >= FraudRiskLevel.High
            })
            .ToListAsync(cancellationToken);

        var totalProcessed = amounts.Sum(a => a.Amount);
        var totalFlagged = amounts.Where(a => a.IsHighRisk).Sum(a => a.Amount);

        return (totalProcessed, totalFlagged);
    }

    private async Task<(int High, int Medium, int Low)> GetRiskBreakdown(
        DateTime today,
        CancellationToken cancellationToken)
    {
        var riskCounts = await _dbContext.Transactions
            .Where(t => t.CreatedAt >= today)
            .GroupBy(t => t.RiskLevel)
            .Select(g => new
            {
                RiskLevel = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var highRisk = riskCounts
            .Where(r => r.RiskLevel == FraudRiskLevel.High || r.RiskLevel == FraudRiskLevel.Critical)
            .Sum(r => r.Count);

        var mediumRisk = riskCounts
            .Where(r => r.RiskLevel == FraudRiskLevel.Medium)
            .Sum(r => r.Count);

        var lowRisk = riskCounts
            .Where(r => r.RiskLevel == FraudRiskLevel.Low)
            .Sum(r => r.Count);

        return (highRisk, mediumRisk, lowRisk);
    }
}
