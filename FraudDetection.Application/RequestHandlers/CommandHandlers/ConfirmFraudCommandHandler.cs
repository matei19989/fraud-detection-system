using MediatR;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.CommandHandlers;

public class ConfirmFraudCommandHandler : IRequestHandler<ConfirmFraudCommand, bool>
{
    private readonly IApplicationDbContext _dbContext;

    public ConfirmFraudCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(ConfirmFraudCommand request, CancellationToken cancellationToken)
    {
        var alert = await _dbContext.FraudAlerts
            .FirstOrDefaultAsync(a => a.Id == request.AlertId, cancellationToken);

        if (alert == null)
            return false;

        alert.ConfirmFraud(request.ConfirmedBy, request.Notes);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}