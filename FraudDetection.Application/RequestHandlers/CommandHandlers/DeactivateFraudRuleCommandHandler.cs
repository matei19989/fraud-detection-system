using MediatR;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.CommandHandlers;

public class DeactivateFraudRuleCommandHandler : IRequestHandler<DeactivateFraudRuleCommand, bool>
{
    private readonly IApplicationDbContext _dbContext;

    public DeactivateFraudRuleCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeactivateFraudRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _dbContext.FraudRules
            .FirstOrDefaultAsync(r => r.Id == request.RuleId, cancellationToken);

        if (rule == null)
            return false;

        rule.Deactivate();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}