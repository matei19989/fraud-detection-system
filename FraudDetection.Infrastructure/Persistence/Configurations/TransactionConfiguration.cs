using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FraudDetection.Domain.Entities;

namespace FraudDetection.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.AccountId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.RiskLevel)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.FraudScore)
            .IsRequired()
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        builder.Property(t => t.TransactionDate)
            .IsRequired();

        builder.Property(t => t.DeviceId)
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        // Configure Amount as owned Money value object
        builder.OwnsOne(t => t.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .IsRequired()
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .IsRequired()
                .HasMaxLength(3);
        });

        // Configure Location as owned value object
        builder.OwnsOne(t => t.Location, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("Latitude")
                .IsRequired();

            location.Property(l => l.Longitude)
                .HasColumnName("Longitude")
                .IsRequired();

            location.Property(l => l.Country)
                .HasColumnName("Country")
                .HasMaxLength(100);

            location.Property(l => l.City)
                .HasColumnName("City")
                .HasMaxLength(100);

            location.Property(l => l.IpAddress)
                .HasColumnName("IpAddress")
                .HasMaxLength(45);
        });

        // Configure Merchant as owned value object
        builder.OwnsOne(t => t.Merchant, merchant =>
        {
            merchant.Property(m => m.MerchantId)
                .HasColumnName("MerchantId")
                .IsRequired()
                .HasMaxLength(100);

            merchant.Property(m => m.MerchantName)
                .HasColumnName("MerchantName")
                .IsRequired()
                .HasMaxLength(200);

            merchant.Property(m => m.Category)
                .HasColumnName("MerchantCategory")
                .IsRequired()
                .HasMaxLength(100);
        });

        // Relationships
        builder.HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .HasPrincipalKey(a => a.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.FraudAlerts)
            .WithOne(a => a.Transaction)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(t => t.DomainEvents);

        builder.ToTable("Transactions");

        builder.HasIndex(t => t.AccountId);
        builder.HasIndex(t => t.TransactionDate);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.RiskLevel);
        builder.HasIndex(t => new { t.AccountId, t.TransactionDate });
    }
}