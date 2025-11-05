using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.QueryHandlers;

public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, List<TransactionDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAllTransactionsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Transactions.AsNoTracking();

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<TransactionStatus>(request.Status, out var status))
        {
            query = query.Where(t => t.Status == status);
        }

        if (!string.IsNullOrEmpty(request.RiskLevel) && Enum.TryParse<FraudRiskLevel>(request.RiskLevel, out var riskLevel))
        {
            query = query.Where(t => t.RiskLevel == riskLevel);
        }

        if (!string.IsNullOrEmpty(request.AccountId))
        {
            query = query.Where(t => t.AccountId.Contains(request.AccountId));
        }

        var transactions = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return transactions.Select(t => new TransactionDto
        {
            Id = t.Id,
            AccountId = t.AccountId,
            Amount = t.Amount.Amount,
            Currency = t.Amount.Currency,
            Type = t.Type.ToString(),
            Status = t.Status.ToString(),
            MerchantId = t.Merchant.MerchantId,
            MerchantName = t.Merchant.MerchantName,
            MerchantCategory = t.Merchant.Category,
            Latitude = t.Location.Latitude,
            Longitude = t.Location.Longitude,
            Country = t.Location.Country,
            City = t.Location.City,
            RiskLevel = t.RiskLevel.ToString(),
            FraudScore = t.FraudScore,
            TransactionDate = t.TransactionDate,
            DeviceId = t.DeviceId,
            CreatedAt = t.CreatedAt
        }).ToList();
    }
}