using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.QueryHandlers;

public class GetAllFraudAlertsQueryHandler : IRequestHandler<GetAllFraudAlertsQuery, List<FraudAlertDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAllFraudAlertsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<FraudAlertDto>> Handle(GetAllFraudAlertsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.FraudAlerts.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<AlertStatus>(request.Status, out var status))
        {
            query = query.Where(a => a.Status == status);
        }

        if (!string.IsNullOrEmpty(request.RiskLevel) && Enum.TryParse<FraudRiskLevel>(request.RiskLevel, out var riskLevel))
        {
            query = query.Where(a => a.RiskLevel == riskLevel);
        }

        // Pagination
        var alerts = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
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