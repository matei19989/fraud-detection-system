using FraudDetection.Domain.Common;
using FraudDetection.Domain.Enums;

namespace FraudDetection.Domain.Events;

public sealed record AlertStatusChangedEvent : IDomainEvent
{
    public Guid EventId { get; init; }
    public DateTime OccurredOn { get; init; }
    public Guid AlertId { get; init; }
    public Guid TransactionId { get; init; }
    public AlertStatus NewStatus { get; init; }
    public string? ReviewedBy { get; init; }
    public string? ReviewNotes { get; init; }

    public AlertStatusChangedEvent(
        Guid alertId,
        Guid transactionId,
        AlertStatus newStatus,
        string? reviewedBy = null,
        string? reviewNotes = null)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        AlertId = alertId;
        TransactionId = transactionId;
        NewStatus = newStatus;
        ReviewedBy = reviewedBy;
        ReviewNotes = reviewNotes;
    }
}