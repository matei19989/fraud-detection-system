using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.QueryHandlers;

public class GetFraudAlertByIdQueryHandler : IRequestHandler<GetFraudAlertByIdQuery, FraudAlertDto?>
{
    private readonly IApplicationDbContext _dbContext;

    public GetFraudAlertByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FraudAlertDto?> Handle(GetFraudAlertByIdQuery request, CancellationToken cancellationToken)
    {
        var alert = await _dbContext.FraudAlerts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AlertId, cancellationToken);

        if (alert == null)
            return null;

        return new FraudAlertDto
        {
            Id = alert.Id,
            TransactionId = alert.TransactionId,
            RuleName = alert.RuleName,
            Status = alert.Status.ToString(),
            RiskLevel = alert.RiskLevel.ToString(),
            Score = alert.Score,
            Message = alert.Message,
            Details = alert.Details,
            ReviewedBy = alert.ReviewedBy,
            ReviewedAt = alert.ReviewedAt,
            CreatedAt = alert.CreatedAt
        };
    }
}