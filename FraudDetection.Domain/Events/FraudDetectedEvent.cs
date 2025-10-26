using FraudDetection.Domain.Common;
using FraudDetection.Domain.Enums;

namespace FraudDetection.Domain.Events;

public sealed record FraudDetectedEvent : IDomainEvent
{
    public Guid EventId { get; init; }
    public DateTime OccurredOn { get; init; }
    public Guid TransactionId { get; init; }
    public string AccountId { get; init; }
    public FraudRiskLevel RiskLevel { get; init; }
    public double FraudScore { get; init; }

    public FraudDetectedEvent(Guid transactionId, string accountId, FraudRiskLevel riskLevel, double fraudScore)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        TransactionId = transactionId;
        AccountId = accountId;
        RiskLevel = riskLevel;
        FraudScore = fraudScore;
    }
}