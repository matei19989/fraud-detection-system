using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Commands;

public record UpdateFraudRuleCommand : IRequest<FraudRuleDto?>
{
    public required Guid RuleId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string RiskLevel { get; init; }
}