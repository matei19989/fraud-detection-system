using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FraudDetection.Domain.Entities;

namespace FraudDetection.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AccountId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(a => a.AccountId)
            .IsUnique();

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(a => a.RegistrationDate)
            .IsRequired();

        builder.Property(a => a.TotalTransactions)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(a => a.AverageTransactionAmount)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(a => a.IsSuspended)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.SuspensionReason)
            .HasMaxLength(500);

        builder.Property(a => a.LastTransactionDate);

        // Configure TotalSpent as owned Money value object
        builder.OwnsOne(a => a.TotalSpent, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalSpentAmount")
                .IsRequired()
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("TotalSpentCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        // Configure LastKnownLocation as owned Location value object
        builder.OwnsOne(a => a.LastKnownLocation, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("LastLocationLatitude");

            location.Property(l => l.Longitude)
                .HasColumnName("LastLocationLongitude");

            location.Property(l => l.Country)
                .HasColumnName("LastLocationCountry")
                .HasMaxLength(100);

            location.Property(l => l.City)
                .HasColumnName("LastLocationCity")
                .HasMaxLength(100);

            location.Property(l => l.IpAddress)
                .HasColumnName("LastLocationIpAddress")
                .HasMaxLength(45);
        });

        // Relationship
        builder.HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId)
            .HasPrincipalKey(a => a.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(a => a.DomainEvents);

        builder.ToTable("Accounts");

        builder.HasIndex(a => a.Email);
        builder.HasIndex(a => a.IsSuspended);
        builder.HasIndex(a => a.LastTransactionDate);
    }
}