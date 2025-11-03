using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class TestApplicationDbContext : DbContext, IApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options)
        : base(options) { }

    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<FraudRule> FraudRules { get; set; } = null!;
    public DbSet<FraudAlert> FraudAlerts { get; set; } = null!;

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>().HasKey(t => t.Id);
        modelBuilder.Entity<Transaction>().OwnsOne(t => t.Amount);
        modelBuilder.Entity<Transaction>().OwnsOne(t => t.Location);
        modelBuilder.Entity<Transaction>().OwnsOne(t => t.Merchant);

        modelBuilder.Entity<Account>().HasKey(a => a.Id);
        modelBuilder.Entity<Account>().OwnsOne(a => a.TotalSpent);
        modelBuilder.Entity<Account>().OwnsOne(a => a.LastKnownLocation);

        modelBuilder.Entity<FraudRule>().HasKey(r => r.Id);
        modelBuilder.Entity<FraudAlert>().HasKey(a => a.Id);
    }
}