using FraudDetection.Domain.Common;
using FraudDetection.Domain.Enums;

namespace FraudDetection.Domain.Events;

public sealed record RuleTriggeredEvent : IDomainEvent
{
    public Guid EventId { get; init; }
    public DateTime OccurredOn { get; init; }
    public Guid RuleId { get; init; }
    public Guid TransactionId { get; init; }
    public string RuleName { get; init; }
    public FraudRiskLevel RiskLevel { get; init; }
    public double Score { get; init; }

    public RuleTriggeredEvent(
        Guid ruleId,
        Guid transactionId,
        string ruleName,
        FraudRiskLevel riskLevel,
        double score)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        RuleId = ruleId;
        TransactionId = transactionId;
        RuleName = ruleName;
        RiskLevel = riskLevel;
        Score = score;
    }
}