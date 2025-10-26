using FraudDetection.Domain.Common;
using FraudDetection.Domain.Enums;

namespace FraudDetection.Domain.Events;

public sealed record TransactionStatusChangedEvent : IDomainEvent
{
    public Guid EventId { get; init; }
    public DateTime OccurredOn { get; init; }
    public Guid TransactionId { get; init; }
    public TransactionStatus NewStatus { get; init; }
    public string? Reason { get; init; }

    public TransactionStatusChangedEvent(Guid transactionId, TransactionStatus newStatus, string? reason = null)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        TransactionId = transactionId;
        NewStatus = newStatus;
        Reason = reason;
    }
}