namespace FraudDetection.Application.DTOs;

public record DashboardStatisticsDto
{
    public int TotalTransactionsToday { get; init; }
    public int TotalTransactionsThisWeek { get; init; }
    public int TotalTransactionsThisMonth { get; init; }
    public int ActiveAlertsCount { get; init; }
    public int ResolvedAlertsToday { get; init; }
    public decimal FraudDetectionRate { get; init; }
    public decimal TotalAmountProcessedToday { get; init; }
    public decimal TotalAmountFlaggedToday { get; init; }
    public int HighRiskTransactionsToday { get; init; }
    public int MediumRiskTransactionsToday { get; init; }
    public int LowRiskTransactionsToday { get; init; }
}