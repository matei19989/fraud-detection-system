using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FraudDetection.Application.RequestHandlers.CommandHandlers;

public class UpdateFraudRuleCommandHandler : IRequestHandler<UpdateFraudRuleCommand, FraudRuleDto?>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public UpdateFraudRuleCommandHandler(IApplicationDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<FraudRuleDto?> Handle(UpdateFraudRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _dbContext.FraudRules
            .FirstOrDefaultAsync(r => r.Id == request.RuleId, cancellationToken);

        if (rule == null)
            return null;

        if (!Enum.TryParse<FraudRiskLevel>(request.RiskLevel, ignoreCase: true, out var riskLevel))
        {
            throw new ArgumentException($"Invalid risk level: {request.RiskLevel}");
        }

        rule.UpdateMetadata(request.Name, request.Description, riskLevel);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _cache.Remove("ActiveFraudRules");

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