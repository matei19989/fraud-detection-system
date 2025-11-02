using Microsoft.EntityFrameworkCore;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Common;
using FraudDetection.Application.Interfaces;

namespace FraudDetection.Infrastructure.Persistence;

public class FraudDetectionDbContext : DbContext, IApplicationDbContext
{
    private readonly IDomainEventDispatcher? _domainEventDispatcher;
    public FraudDetectionDbContext(DbContextOptions<FraudDetectionDbContext> options,
        IDomainEventDispatcher? domainEventDispatcher)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FraudAlert> FraudAlerts => Set<FraudAlert>();
    public DbSet<FraudRule> FraudRules => Set<FraudRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FraudDetectionDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Publish domain events before saving
        var entities = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Clear events before saving
        entities.ForEach(e => e.ClearDomainEvents());

        // Propagate cancellation token to base SaveChangesAsync
        var result = await base.SaveChangesAsync(cancellationToken);

        if (domainEvents.Count != 0 && _domainEventDispatcher != null)
        {
            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
        }

        return result;
    }
}