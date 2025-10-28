using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;

namespace FraudDetection.Application.RequestHandlers.CommandHandlers;

public class CreateFraudRuleCommandHandler : IRequestHandler<CreateFraudRuleCommand, FraudRuleDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateFraudRuleCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FraudRuleDto> Handle(CreateFraudRuleCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<FraudRiskLevel>(request.RiskLevel, ignoreCase: true, out var riskLevel))
        {
            throw new ArgumentException($"Invalid risk level: {request.RiskLevel}");
        }

        var rule = new FraudRule(
            request.Name,
            request.Description,
            riskLevel,
            request.RuleType,
            request.ConditionsJson,
            request.Priority);

        await _dbContext.FraudRules.AddAsync(rule, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new FraudRuleDto
        {
            Id = rule.Id,
            Name = rule.Name,
            Description = rule.Description,
            IsActive = rule.IsActive,
            RiskLevel = rule.RiskLevel.ToString(),
            Priority = rule.Priority,
            RuleType = rule.RuleType,
            TimesTriggered = rule.TimesTriggered,
            LastTriggeredAt = rule.LastTriggeredAt
        };
    }
}