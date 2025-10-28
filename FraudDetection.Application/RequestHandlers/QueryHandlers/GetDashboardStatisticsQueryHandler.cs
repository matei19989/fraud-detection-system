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
        var today = DateTime.UtcNow.Date;
        var weekAgo = today.AddDays(-7);
        var monthAgo = today.AddDays(-30);

        var todayTransactions = await _dbContext.Transactions
            .Where(t => t.CreatedAt >= today)
            .ToListAsync(cancellationToken);

        var weekTransactions = await _dbContext.Transactions
            .CountAsync(t => t.CreatedAt >= weekAgo, cancellationToken);

        var monthTransactions = await _dbContext.Transactions
            .CountAsync(t => t.CreatedAt >= monthAgo, cancellationToken);

        var activeAlerts = await _dbContext.FraudAlerts
            .CountAsync(a => a.Status == AlertStatus.New || a.Status == AlertStatus.Investigating, cancellationToken);

        var resolvedToday = await _dbContext.FraudAlerts
            .CountAsync(a => a.Status == AlertStatus.Resolved && a.UpdatedAt >= today, cancellationToken);

        var totalProcessedToday = todayTransactions.Sum(t => t.Amount.Amount);
        var flaggedToday = todayTransactions.Where(t => t.RiskLevel >= FraudRiskLevel.High).Sum(t => t.Amount.Amount);

        var highRiskToday = todayTransactions.Count(t => t.RiskLevel == FraudRiskLevel.High || t.RiskLevel == FraudRiskLevel.Critical);
        var mediumRiskToday = todayTransactions.Count(t => t.RiskLevel == FraudRiskLevel.Medium);
        var lowRiskToday = todayTransactions.Count(t => t.RiskLevel == FraudRiskLevel.Low);

        var fraudDetectionRate = todayTransactions.Count > 0
            ? (decimal)highRiskToday / todayTransactions.Count * 100
            : 0;

        return new DashboardStatisticsDto
        {
            TotalTransactionsToday = todayTransactions.Count,
            TotalTransactionsThisWeek = weekTransactions,
            TotalTransactionsThisMonth = monthTransactions,
            ActiveAlertsCount = activeAlerts,
            ResolvedAlertsToday = resolvedToday,
            FraudDetectionRate = fraudDetectionRate,
            TotalAmountProcessedToday = totalProcessedToday,
            TotalAmountFlaggedToday = flaggedToday,
            HighRiskTransactionsToday = highRiskToday,
            MediumRiskTransactionsToday = mediumRiskToday,
            LowRiskTransactionsToday = lowRiskToday
        };
    }
}