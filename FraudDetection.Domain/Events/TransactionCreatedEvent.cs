using FraudDetection.Domain.Common;
using FraudDetection.Domain.ValueObjects;

namespace FraudDetection.Domain.Events;

public sealed record TransactionCreatedEvent : IDomainEvent
{
    public Guid EventId { get; init; }
    public DateTime OccurredOn { get; init; }
    public Guid TransactionId { get; init; }
    public string AccountId { get; init; }
    public Money Amount { get; init; }
    public DateTime TransactionDate { get; init; }

    public TransactionCreatedEvent(Guid transactionId, string accountId, Money amount, DateTime transactionDate)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        TransactionId = transactionId;
        AccountId = accountId;
        Amount = amount;
        TransactionDate = transactionDate;
    }
}