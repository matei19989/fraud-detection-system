using MediatR;
using Microsoft.Extensions.Logging;
using FraudDetection.Domain.Events;
using FraudDetection.Application.Interfaces;

namespace FraudDetection.Application.EventHandlers;

public class TransactionCreatedEventHandler : INotificationHandler<TransactionCreatedEvent>
{
    private readonly IRealtimeNotificationService _notificationService;
    private readonly ILogger<TransactionCreatedEventHandler> _logger;

    public TransactionCreatedEventHandler(
        IRealtimeNotificationService notificationService,
        ILogger<TransactionCreatedEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing TransactionCreatedEvent for Transaction ID: {TransactionId}, Account: {AccountId}",
            notification.TransactionId,
            notification.AccountId);

        var eventData = new
        {
            transactionId = notification.TransactionId,
            accountId = notification.AccountId,
            amount = notification.Amount.Amount,
            currency = notification.Amount.Currency,
            transactionDate = notification.TransactionDate
        };

        // Broadcast to all clients
        await _notificationService.BroadcastAsync("TransactionCreated", eventData, cancellationToken);

        // Also send to specific account group
        await _notificationService.SendToGroupAsync(
            $"account-{notification.AccountId}",
            "TransactionCreated",
            eventData,
            cancellationToken);

        _logger.LogInformation(
            "TransactionCreatedEvent processed and broadcast for Transaction ID: {TransactionId}",
            notification.TransactionId);
    }
}