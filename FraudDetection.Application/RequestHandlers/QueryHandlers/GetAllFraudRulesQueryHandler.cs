using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.QueryHandlers;

public class GetAllFraudRulesQueryHandler : IRequestHandler<GetAllFraudRulesQuery, List<FraudRuleDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAllFraudRulesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<FraudRuleDto>> Handle(GetAllFraudRulesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.FraudRules.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(request.RuleType))
        {
            query = query.Where(r => r.RuleType == request.RuleType);
        }

        var rules = await query
            .OrderByDescending(r => r.Priority)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);

        return rules.Select(r => new FraudRuleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            IsActive = r.IsActive,
            RiskLevel = r.RiskLevel.ToString(),
            Priority = r.Priority,
            RuleType = r.RuleType,
            TimesTriggered = r.TimesTriggered,
            LastTriggeredAt = r.LastTriggeredAt
        }).ToList();
    }
}