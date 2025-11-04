using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.QueryHandlers;

public class GetRecentAlertsQueryHandler : IRequestHandler<GetRecentAlertsQuery, List<FraudAlertDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetRecentAlertsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<FraudAlertDto>> Handle(GetRecentAlertsQuery request, CancellationToken cancellationToken)
    {
        var alerts = await _dbContext.FraudAlerts
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Take(request.Count)
            .ToListAsync(cancellationToken);

        return alerts.Select(a => new FraudAlertDto
        {
            Id = a.Id,
            TransactionId = a.TransactionId,
            RuleName = a.RuleName,
            Status = a.Status.ToString(),
            RiskLevel = a.RiskLevel.ToString(),
            Score = a.Score,
            Message = a.Message,
            Details = a.Details,
            ReviewedBy = a.ReviewedBy,
            ReviewedAt = a.ReviewedAt,
            CreatedAt = a.CreatedAt
        }).ToList();
    }
}