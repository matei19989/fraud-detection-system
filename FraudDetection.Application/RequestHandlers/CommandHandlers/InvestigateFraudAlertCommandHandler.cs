using MediatR;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.CommandHandlers;

public class InvestigateFraudAlertCommandHandler : IRequestHandler<InvestigateFraudAlertCommand, bool>
{
    private readonly IApplicationDbContext _dbContext;

    public InvestigateFraudAlertCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(InvestigateFraudAlertCommand request, CancellationToken cancellationToken)
    {
        var alert = await _dbContext.FraudAlerts
            .FirstOrDefaultAsync(a => a.Id == request.AlertId, cancellationToken);

        if (alert == null)
            return false;

        alert.Investigate(request.InvestigatedBy);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}