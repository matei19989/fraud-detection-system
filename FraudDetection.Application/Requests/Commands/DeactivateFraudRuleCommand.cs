using MediatR;

namespace FraudDetection.Application.Requests.Commands;

public record DeactivateFraudRuleCommand : IRequest<bool>
{
    public required Guid RuleId { get; init; }
}