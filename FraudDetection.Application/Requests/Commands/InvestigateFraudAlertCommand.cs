using MediatR;

namespace FraudDetection.Application.Requests.Commands;

public record InvestigateFraudAlertCommand : IRequest<bool>
{
    public required Guid AlertId { get; init; }
    public required string InvestigatedBy { get; init; }
}