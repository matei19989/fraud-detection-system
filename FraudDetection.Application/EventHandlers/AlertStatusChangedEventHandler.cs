using MediatR;
using Microsoft.Extensions.Logging;
using FraudDetection.Domain.Events;
using FraudDetection.Application.Interfaces;

namespace FraudDetection.Application.EventHandlers;

public class AlertStatusChangedEventHandler : INotificationHandler<AlertStatusChangedEvent>
{
    private readonly IRealtimeNotificationService _notificationService;
    private readonly ILogger<AlertStatusChangedEventHandler> _logger;

    public AlertStatusChangedEventHandler(
        IRealtimeNotificationService notificationService,
        ILogger<AlertStatusChangedEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(AlertStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Alert status changed - Alert ID: {AlertId}, Transaction ID: {TransactionId}, New Status: {NewStatus}",
            notification.AlertId,
            notification.TransactionId,
            notification.NewStatus);

        var statusData = new
        {
            alertId = notification.AlertId,
            transactionId = notification.TransactionId,
            newStatus = notification.NewStatus.ToString(),
            reviewedBy = notification.ReviewedBy,
            reviewNotes = notification.ReviewNotes,
            occurredOn = notification.OccurredOn
        };

        await _notificationService.BroadcastAsync("AlertStatusChanged", statusData, cancellationToken);

        _logger.LogInformation(
            "AlertStatusChangedEvent broadcast for Alert ID: {AlertId}",
            notification.AlertId);
    }
}