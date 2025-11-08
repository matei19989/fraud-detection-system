using MediatR;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FraudDetection.Application.RequestHandlers.CommandHandlers;

public class ActivateFraudRuleCommandHandler : IRequestHandler<ActivateFraudRuleCommand, bool>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public ActivateFraudRuleCommandHandler(IApplicationDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<bool> Handle(ActivateFraudRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _dbContext.FraudRules
            .FirstOrDefaultAsync(r => r.Id == request.RuleId, cancellationToken);

        if (rule == null)
            return false;

        rule.Activate();
        await _dbContext.SaveChangesAsync(cancellationToken);

        _cache.Remove("ActiveFraudRules");

        return true;
    }
}