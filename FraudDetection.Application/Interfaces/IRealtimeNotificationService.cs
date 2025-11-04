namespace FraudDetection.Application.Interfaces;

public interface IRealtimeNotificationService
{
    Task BroadcastAsync(string eventName, object data, CancellationToken cancellationToken = default);

    Task SendToGroupAsync(string groupName, string eventName, object data, CancellationToken cancellationToken = default);

    Task SendToUserAsync(string userId, string eventName, object data, CancellationToken cancellationToken = default);
}