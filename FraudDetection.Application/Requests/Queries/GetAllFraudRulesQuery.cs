using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Queries;

public record GetAllFraudRulesQuery : IRequest<List<FraudRuleDto>>
{
    public bool? IsActive { get; init; }
    public string? RuleType { get; init; }
}