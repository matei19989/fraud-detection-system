using MediatR;

namespace FraudDetection.Application.Requests.Commands;

public record UpdateFraudRulePriorityCommand : IRequest<bool>
{
    public required Guid RuleId { get; init; }
    public required int NewPriority { get; init; }
}