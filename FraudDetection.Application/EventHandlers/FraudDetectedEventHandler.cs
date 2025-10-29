using MediatR;
using Microsoft.Extensions.Logging;
using FraudDetection.Domain.Events;
using FraudDetection.Application.Interfaces;

namespace FraudDetection.Application.EventHandlers;

public class FraudDetectedEventHandler : INotificationHandler<FraudDetectedEvent>
{
    private readonly IRealtimeNotificationService _notificationService;
    private readonly ILogger<FraudDetectedEventHandler> _logger;

    public FraudDetectedEventHandler(
        IRealtimeNotificationService notificationService,
        ILogger<FraudDetectedEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(FraudDetectedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "FRAUD DETECTED - Transaction ID: {TransactionId}, Account: {AccountId}, Risk Level: {RiskLevel}, Score: {Score}",
            notification.TransactionId,
            notification.AccountId,
            notification.RiskLevel,
            notification.FraudScore);

        var alertData = new
        {
            transactionId = notification.TransactionId,
            accountId = notification.AccountId,
            riskLevel = notification.RiskLevel.ToString(),
            fraudScore = notification.FraudScore,
            occurredOn = notification.OccurredOn
        };

        // Broadcast urgent fraud alert to all clients
        await _notificationService.BroadcastAsync("FraudDetected", alertData, cancellationToken);

        _logger.LogInformation(
            "FraudDetectedEvent broadcast for Transaction ID: {TransactionId}",
            notification.TransactionId);
    }
}