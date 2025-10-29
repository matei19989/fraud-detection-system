using MediatR;
using Microsoft.Extensions.Logging;
using FraudDetection.Domain.Events;
using FraudDetection.Application.Interfaces;

namespace FraudDetection.Application.EventHandlers;

public class TransactionStatusChangedEventHandler : INotificationHandler<TransactionStatusChangedEvent>
{
    private readonly IRealtimeNotificationService _notificationService;
    private readonly ILogger<TransactionStatusChangedEventHandler> _logger;

    public TransactionStatusChangedEventHandler(
        IRealtimeNotificationService notificationService,
        ILogger<TransactionStatusChangedEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(TransactionStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Transaction status changed - Transaction ID: {TransactionId}, New Status: {NewStatus}",
            notification.TransactionId,
            notification.NewStatus);

        var statusData = new
        {
            transactionId = notification.TransactionId,
            newStatus = notification.NewStatus.ToString(),
            reason = notification.Reason,
            occurredOn = notification.OccurredOn
        };

        await _notificationService.BroadcastAsync("TransactionStatusChanged", statusData, cancellationToken);
    }
}