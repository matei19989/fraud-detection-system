using Microsoft.AspNetCore.SignalR;

namespace FraudDetection.API.Hubs;

public class FraudHub : Hub
{
    //send transaction created notification to all clients
    public async Task NotifyTransactionCreated(object transaction)
    {
        await Clients.All.SendAsync("TransactionCreated", transaction);
    }

    //send fraud detected notification to all clients
    public async Task NotifyFraudDetected(object fraudAlert)
    {
        await Clients.All.SendAsync("FraudDetected", fraudAlert);
    }

    //send alert status change notification to all clients
    public async Task NotifyAlertStatusChanged(object alert)
    {
        await Clients.All.SendAsync("AlertStatusChanged", alert);
    }

    //subscribe to specific account notifications
    public async Task SubscribeToAccount(string accountId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"account-{accountId}");
    }

    //unsubscribe from specific account notifications
    public async Task UnsubscribeFromAccount(string accountId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"account-{accountId}");
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}