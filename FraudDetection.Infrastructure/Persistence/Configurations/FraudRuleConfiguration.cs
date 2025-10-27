using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FraudDetection.Domain.Entities;

namespace FraudDetection.Infrastructure.Persistence.Configurations;

public class FraudRuleConfiguration : IEntityTypeConfiguration<FraudRule>
{
    public void Configure(EntityTypeBuilder<FraudRule> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.RiskLevel)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.Priority)
            .IsRequired();

        builder.Property(r => r.RuleType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.ConditionsJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(r => r.TimesTriggered)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(r => r.LastTriggeredAt);

        builder.HasMany(r => r.Alerts)
            .WithOne(a => a.Rule)
            .HasForeignKey(a => a.RuleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(r => r.DomainEvents);

        builder.ToTable("FraudRules");

        builder.HasIndex(r => r.IsActive);
        builder.HasIndex(r => r.Priority);
        builder.HasIndex(r => r.RuleType);
    }
}