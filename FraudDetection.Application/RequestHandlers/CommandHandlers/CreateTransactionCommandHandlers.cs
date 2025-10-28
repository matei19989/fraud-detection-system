using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using FraudDetection.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Application.RequestHandlers.CommandHandlers;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateTransactionCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var amount = new Money(request.Amount, request.Currency);
        var merchant = new MerchantInfo(request.MerchantId, request.MerchantName, request.MerchantCategory);
        var location = new Location(
            request.Latitude,
            request.Longitude,
            request.Country,
            request.City,
            request.IpAddress);

        if (!Enum.TryParse<TransactionType>(request.Type, ignoreCase: true, out var transactionType))
        {
            throw new ArgumentException($"Invalid transaction type: {request.Type}");
        }

        var transaction = new Transaction(
            request.AccountId,
            amount,
            transactionType,
            merchant,
            location,
            request.DeviceId,
            request.Description);

        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.AccountId == request.AccountId, cancellationToken);

        if (account != null)
        {
            account.UpdateTransactionStatistics(amount);
            account.UpdateLastKnownLocation(location);
        }

        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

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