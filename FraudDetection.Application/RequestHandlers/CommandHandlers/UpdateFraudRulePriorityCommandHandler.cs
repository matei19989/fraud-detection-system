using MediatR;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.CommandHandlers;

public class UpdateFraudRulePriorityCommandHandler : IRequestHandler<UpdateFraudRulePriorityCommand, bool>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateFraudRulePriorityCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(UpdateFraudRulePriorityCommand request, CancellationToken cancellationToken)
    {
        var rule = await _dbContext.FraudRules
            .FirstOrDefaultAsync(r => r.Id == request.RuleId, cancellationToken);

        if (rule == null)
            return false;

        rule.UpdatePriority(request.NewPriority);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}