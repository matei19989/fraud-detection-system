using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.QueryHandlers;

public class GetFraudRuleByIdQueryHandler : IRequestHandler<GetFraudRuleByIdQuery, FraudRuleDto?>
{
    private readonly IApplicationDbContext _dbContext;

    public GetFraudRuleByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FraudRuleDto?> Handle(GetFraudRuleByIdQuery request, CancellationToken cancellationToken)
    {
        var rule = await _dbContext.FraudRules
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RuleId, cancellationToken);

        if (rule == null)
            return null;

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