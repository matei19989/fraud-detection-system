using MediatR;

namespace FraudDetection.Application.Requests.Commands;

public record ResolveFraudAlertCommand : IRequest<bool>
{
    public required Guid AlertId { get; init; }
    public required string ResolvedBy { get; init; }
    public required string Notes { get; init; }
}