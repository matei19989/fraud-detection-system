using FraudDetection.Application.Interfaces;

namespace FraudDetection.Infrastructure.Services;

public class RealtimeNotificationService : IRealtimeNotificationService
{
    private readonly IHubContextWrapper _hubContextWrapper;

    public RealtimeNotificationService(IHubContextWrapper hubContextWrapper)
    {
        _hubContextWrapper = hubContextWrapper;
    }

    public async Task BroadcastAsync(string eventName, object data, CancellationToken cancellationToken = default)
    {
        await _hubContextWrapper.SendToAllAsync(eventName, data, cancellationToken);
    }

    public async Task SendToGroupAsync(string groupName, string eventName, object data, CancellationToken cancellationToken = default)
    {
        await _hubContextWrapper.SendToGroupAsync(groupName, eventName, data, cancellationToken);
    }

    public async Task SendToUserAsync(string userId, string eventName, object data, CancellationToken cancellationToken = default)
    {
        await _hubContextWrapper.SendToUserAsync(userId, eventName, data, cancellationToken);
    }
}