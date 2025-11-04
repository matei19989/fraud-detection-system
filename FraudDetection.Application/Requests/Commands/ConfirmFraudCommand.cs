using MediatR;

namespace FraudDetection.Application.Requests.Commands;

public record ConfirmFraudCommand : IRequest<bool>
{
    public required Guid AlertId { get; init; }
    public required string ConfirmedBy { get; init; }
    public required string Notes { get; init; }
}