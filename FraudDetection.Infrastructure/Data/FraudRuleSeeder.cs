using System.Text.Json;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using FraudDetection.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FraudDetection.Application.DTOs.RuleConditions;

namespace FraudDetection.Infrastructure.Data;

public class FraudRuleSeeder
{
    private readonly FraudDetectionDbContext _context;
    private readonly ILogger<FraudRuleSeeder> _logger;

    public FraudRuleSeeder(
        FraudDetectionDbContext context,
        ILogger<FraudRuleSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.FraudRules.AnyAsync())
        {
            _logger.LogInformation("Fraud rules already exist, skipping seed");
            return;
        }

        _logger.LogInformation("Seeding fraud detection rules...");

        var rules = new List<FraudRule>
        {
            CreateHighValueTransactionRule(),
            CreateVelocityRule(),
            CreateLocationAnomalyRule(),
            CreateNewAccountHighValueRule(),
            CreateHighRiskMerchantRule(),
            CreateNightTimeTransactionRule(),
            CreateAmountDeviationRule()
        };

        await _context.FraudRules.AddRangeAsync(rules);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully seeded {Count} fraud detection rules", rules.Count);
    }

    private FraudRule CreateHighValueTransactionRule()
    {
        var condition = new AmountThresholdCondition
        {
            Threshold = 5000,
            Operator = "GreaterThan"
        };

        return new FraudRule(
            "High Value Transaction",
            "Triggers when transaction amount exceeds $5,000",
            FraudRiskLevel.Medium,
            "AmountBased",
            SerializeCondition(condition),
            priority: 8);
    }

    private FraudRule CreateVelocityRule()
    {
        var condition = new VelocityCondition
        {
            TransactionCount = 5,
            TimeWindowMinutes = 10
        };

        return new FraudRule(
            "High Transaction Velocity",
            "Triggers when account makes 5+ transactions within 10 minutes",
            FraudRiskLevel.High,
            "VelocityBased",
            SerializeCondition(condition),
            priority: 10);
    }

    private FraudRule CreateLocationAnomalyRule()
    {
        var condition = new LocationAnomalyCondition
        {
            MaxDistanceKm = 500,
            TimeWindowMinutes = 60
        };

        return new FraudRule(
            "Impossible Travel Detected",
            "Triggers when transactions occur more than 500km apart within 1 hour",
            FraudRiskLevel.Critical,
            "LocationBased",
            SerializeCondition(condition),
            priority: 10);
    }

    private FraudRule CreateNewAccountHighValueRule()
    {
        var condition = new NewAccountCondition
        {
            AccountAgeDays = 7,
            MaxTransactionAmount = 1000
        };

        return new FraudRule(
            "New Account Large Transaction",
            "Triggers when accounts less than 7 days old make transactions over $1,000",
            FraudRiskLevel.High,
            "AccountBased",
            SerializeCondition(condition),
            priority: 9);
    }

    private FraudRule CreateHighRiskMerchantRule()
    {
        var condition = new UnusualMerchantCondition
        {
            HighRiskCategories = new List<string>
            {
                "Gambling",
                "Cryptocurrency",
                "Adult Entertainment",
                "Money Transfer",
                "Wire Transfer"
            }
        };

        return new FraudRule(
            "High Risk Merchant Category",
            "Triggers for transactions with high-risk merchant categories",
            FraudRiskLevel.Medium,
            "MerchantBased",
            SerializeCondition(condition),
            priority: 6);
    }

    private FraudRule CreateNightTimeTransactionRule()
    {
        var condition = new TimeOfDayCondition
        {
            StartHour = 2,
            EndHour = 4
        };

        return new FraudRule(
            "Unusual Transaction Time",
            "Triggers for transactions between 2 AM and 4 AM (high fraud risk window)",
            FraudRiskLevel.Low,
            "TimeBased",
            SerializeCondition(condition),
            priority: 4);
    }

    private FraudRule CreateAmountDeviationRule()
    {
        var condition = new AmountDeviationCondition
        {
            StandardDeviationMultiplier = 3.0,
            MinimumTransactionCount = 5
        };

        return new FraudRule(
            "Unusual Transaction Amount",
            "Triggers when transaction amount significantly deviates from account's average",
            FraudRiskLevel.Medium,
            "PatternBased",
            SerializeCondition(condition),
            priority: 7);
    }

    private string SerializeCondition(RuleCondition condition)
    {
        return JsonSerializer.Serialize(condition, condition.GetType(), new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}