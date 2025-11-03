namespace FraudDetection.Application.Interfaces;

public interface IHubContextWrapper
{
    Task SendToAllAsync(string method, object data, CancellationToken cancellationToken = default);
    Task SendToGroupAsync(string groupName, string method, object data, CancellationToken cancellationToken = default);
    Task SendToUserAsync(string userId, string method, object data, CancellationToken cancellationToken = default);
}