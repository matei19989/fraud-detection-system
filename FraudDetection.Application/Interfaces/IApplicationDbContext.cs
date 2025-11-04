using Microsoft.EntityFrameworkCore;
using FraudDetection.Domain.Entities;

namespace FraudDetection.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<FraudAlert> FraudAlerts { get; }
    DbSet<FraudRule> FraudRules { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
