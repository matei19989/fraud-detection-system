using MediatR;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Entities;

namespace FraudDetection.Application.RequestHandlers.CommandHandlers;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateAccountCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account(
            request.AccountId,
            request.Email,
            request.PhoneNumber);

        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AccountDto
        {
            Id = account.Id,
            AccountId = account.AccountId,
            Email = account.Email,
            PhoneNumber = account.PhoneNumber,
            TotalTransactions = account.TotalTransactions,
            AverageTransactionAmount = account.AverageTransactionAmount,
            IsSuspended = account.IsSuspended,
            LastTransactionDate = account.LastTransactionDate
        };
    }
}