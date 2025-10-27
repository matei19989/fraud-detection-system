using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FraudDetection.Domain.Entities;

namespace FraudDetection.Infrastructure.Persistence.Configurations;

public class FraudAlertConfiguration : IEntityTypeConfiguration<FraudAlert>
{
    public void Configure(EntityTypeBuilder<FraudAlert> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.TransactionId)
            .IsRequired();

        builder.Property(a => a.RuleId);

        builder.Property(a => a.RuleName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.RiskLevel)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.Score)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(a => a.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Details)
            .HasMaxLength(2000);

        builder.Property(a => a.ReviewedBy)
            .HasMaxLength(200);

        builder.Property(a => a.ReviewedAt);

        builder.Property(a => a.ReviewNotes)
            .HasMaxLength(2000);

        builder.HasOne(a => a.Transaction)
            .WithMany(t => t.FraudAlerts)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Rule)
            .WithMany(r => r.Alerts)
            .HasForeignKey(a => a.RuleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(a => a.DomainEvents);

        builder.ToTable("FraudAlerts");

        builder.HasIndex(a => a.TransactionId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.RiskLevel);
        builder.HasIndex(a => a.CreatedAt);
    }
}