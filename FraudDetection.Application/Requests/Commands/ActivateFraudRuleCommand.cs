using MediatR;

namespace FraudDetection.Application.Requests.Commands;

public record ActivateFraudRuleCommand : IRequest<bool>
{
    public required Guid RuleId { get; init; }
}