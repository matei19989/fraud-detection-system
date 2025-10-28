using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.QueryHandlers;

public class GetTransactionsByAccountQueryHandler : IRequestHandler<GetTransactionsByAccountQuery, List<TransactionDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetTransactionsByAccountQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TransactionDto>> Handle(GetTransactionsByAccountQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == request.AccountId)
            .OrderByDescending(t => t.TransactionDate)
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