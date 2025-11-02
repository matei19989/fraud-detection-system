using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IPublisher publisher, ILogger<DomainEventDispatcher> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Dispatching domain event: {EventType} with ID: {EventId}",
            domainEvent.GetType().Name,
            domainEvent.EventId);

        try
        {
            var notification = domainEvent as INotification;

            if (notification != null)
            {
                await _publisher.Publish(notification, cancellationToken);
            }
            else
            {
                _logger.LogWarning(
                    "Domain event {EventType} does not implement INotification and cannot be published",
                    domainEvent.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error dispatching domain event: {EventType} with ID: {EventId}",
                domainEvent.GetType().Name,
                domainEvent.EventId);
        }
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, cancellationToken);
        }
    }
}