using Microsoft.AspNetCore.SignalR;
using FraudDetection.Application.Interfaces;
using FraudDetection.API.Hubs;

namespace FraudDetection.API.Services;

public class FraudHubContextWrapper : IHubContextWrapper
{
    private readonly IHubContext<FraudHub> _hubContext;

    public FraudHubContextWrapper(IHubContext<FraudHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToAllAsync(string method, object data, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.SendAsync(method, data, cancellationToken);
    }

    public async Task SendToGroupAsync(string groupName, string method, object data, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Group(groupName).SendAsync(method, data, cancellationToken);
    }

    public async Task SendToUserAsync(string userId, string method, object data, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.User(userId).SendAsync(method, data, cancellationToken);
    }
}