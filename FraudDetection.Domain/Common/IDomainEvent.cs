using MediatR;

namespace FraudDetection.Domain.Common;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}