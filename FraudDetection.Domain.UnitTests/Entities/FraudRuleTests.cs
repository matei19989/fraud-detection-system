using FluentAssertions;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using Xunit;

namespace FraudDetection.Domain.UnitTests.Entities;

public class FraudRuleTests
{
    private const string ValidConditionsJson = "{\"amountThreshold\": 1000, \"currency\": \"USD\"}";

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateFraudRule()
    {
        var rule = new FraudRule(
            "High Value Transaction",
            "Detects transactions above threshold",
            FraudRiskLevel.High,
            "AmountThreshold",
            ValidConditionsJson,
            5);

        rule.Name.Should().Be("High Value Transaction");
        rule.Description.Should().Be("Detects transactions above threshold");
        rule.RiskLevel.Should().Be(FraudRiskLevel.High);
        rule.RuleType.Should().Be("AmountThreshold");
        rule.ConditionsJson.Should().Be(ValidConditionsJson);
        rule.Priority.Should().Be(5);
        rule.IsActive.Should().BeTrue();
        rule.TimesTriggered.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyName_ShouldThrowArgumentException(string? name)
    {
        Action act = () => new FraudRule(
            name!,
            "Description",
            FraudRiskLevel.Medium,
            "TestType",
            ValidConditionsJson);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Name cannot be null or empty*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidPriority_ShouldThrowArgumentException(int priority)
    {
        Action act = () => new FraudRule(
            "Test Rule",
            "Description",
            FraudRiskLevel.Medium,
            "TestType",
            ValidConditionsJson,
            priority);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Priority must be at least 1*");
    }

    [Fact]
    public void Activate_WhenInactive_ShouldSetIsActiveToTrue()
    {
        var rule = new FraudRule("Test Rule", "Description", FraudRiskLevel.Medium, "TestType", ValidConditionsJson);
        rule.Deactivate();

        rule.Activate();

        rule.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldSetIsActiveToFalse()
    {
        var rule = new FraudRule("Test Rule", "Description", FraudRiskLevel.Medium, "TestType", ValidConditionsJson);

        rule.Deactivate();

        rule.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdatePriority_WithValidPriority_ShouldUpdatePriority()
    {
        var rule = new FraudRule("Test Rule", "Description", FraudRiskLevel.Medium, "TestType", ValidConditionsJson, 5);

        rule.UpdatePriority(10);

        rule.Priority.Should().Be(10);
    }

    [Fact]
    public void RecordTrigger_ShouldIncrementTimesTriggered()
    {
        var rule = new FraudRule("Test Rule", "Description", FraudRiskLevel.Medium, "TestType", ValidConditionsJson);

        rule.RecordTrigger();
        rule.RecordTrigger();
        rule.RecordTrigger();

        rule.TimesTriggered.Should().Be(3);
        rule.LastTriggeredAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateMetadata_WithValidParameters_ShouldUpdateAllFields()
    {
        var rule = new FraudRule("Old Name", "Old Description", FraudRiskLevel.Low, "TestType", ValidConditionsJson);

        rule.UpdateMetadata("New Name", "New Description", FraudRiskLevel.Critical);

        rule.Name.Should().Be("New Name");
        rule.Description.Should().Be("New Description");
        rule.RiskLevel.Should().Be(FraudRiskLevel.Critical);
    }
}
