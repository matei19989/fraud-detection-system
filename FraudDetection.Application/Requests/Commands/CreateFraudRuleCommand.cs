using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Commands;

public record CreateFraudRuleCommand : IRequest<FraudRuleDto>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string RiskLevel { get; init; }
    public required string RuleType { get; init; }
    public required string ConditionsJson { get; init; }
    public int Priority { get; init; } = 1;
}