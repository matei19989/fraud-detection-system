using FraudDetection.Domain.Entities;

namespace FraudDetection.Application.Interfaces;

public interface IFraudDetectionService
{
    Task<FraudAnalysisResult> AnalyzeTransactionAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);
}

public record FraudAnalysisResult
{
    public double FraudScore { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public List<FraudAlert> AlertsCreated { get; init; } = new();
    public List<RuleEvaluationResult> RuleResults { get; init; } = new();
    public bool IsFraudulent => FraudScore >= 75;
}

public record RuleEvaluationResult
{
    public Guid RuleId { get; init; }
    public string RuleName { get; init; } = string.Empty;
    public bool Triggered { get; init; }
    public double Score { get; init; }
    public string? Details { get; init; }
}