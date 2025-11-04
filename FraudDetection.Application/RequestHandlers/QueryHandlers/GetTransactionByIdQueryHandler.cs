using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.QueryHandlers;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly IApplicationDbContext _dbContext;

    public GetTransactionByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _dbContext.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TransactionId, cancellationToken);

        if (transaction == null)
            return null;

        return new TransactionDto
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            Amount = transaction.Amount.Amount,
            Currency = transaction.Amount.Currency,
            Type = transaction.Type.ToString(),
            Status = transaction.Status.ToString(),
            MerchantId = transaction.Merchant.MerchantId,
            MerchantName = transaction.Merchant.MerchantName,
            MerchantCategory = transaction.Merchant.Category,
            Latitude = transaction.Location.Latitude,
            Longitude = transaction.Location.Longitude,
            Country = transaction.Location.Country,
            City = transaction.Location.City,
            RiskLevel = transaction.RiskLevel.ToString(),
            FraudScore = transaction.FraudScore,
            TransactionDate = transaction.TransactionDate,
            DeviceId = transaction.DeviceId,
            CreatedAt = transaction.CreatedAt
        };
    }
}