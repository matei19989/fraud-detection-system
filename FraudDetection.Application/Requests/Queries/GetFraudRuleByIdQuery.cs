using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Queries;

public record GetFraudRuleByIdQuery : IRequest<FraudRuleDto?>
{
    public required Guid RuleId { get; init; }
}