using Microsoft.AspNetCore.SignalR;
using FraudDetection.Application.Interfaces;
using FraudDetection.API.Hubs;

namespace FraudDetection.Infrastructure.Services;

public class SignalRNotificationService : IRealtimeNotificationService
{
    private readonly IHubContext<FraudHub> _hubContext;

    public SignalRNotificationService(IHubContext<FraudHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastAsync(string eventName, object data, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync(eventName, data, cancellationToken);
    }

    public async Task SendToGroupAsync(string groupName, string eventName, object data, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(groupName).SendAsync(eventName, data, cancellationToken);
    }

    public async Task SendToUserAsync(string userId, string eventName, object data, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userId).SendAsync(eventName, data, cancellationToken);
    }
}