using FluentAssertions;
using FraudDetection.Domain.Common;
using FraudDetection.Domain.Enums;
using FraudDetection.Domain.Events;
using FraudDetection.Domain.ValueObjects;
using Xunit;

namespace FraudDetection.Domain.UnitTests.Events;

public class DomainEventsTests
{
    [Fact]
    public void TransactionCreatedEvent_ShouldInitializeWithCorrectValues()
    {
        var transactionId = Guid.NewGuid();
        var accountId = "ACC123";
        var amount = new Money(100, "USD");
        var transactionDate = DateTime.UtcNow;
        var before = DateTime.UtcNow;

        var @event = new TransactionCreatedEvent(transactionId, accountId, amount, transactionDate);
        var after = DateTime.UtcNow;

        @event.EventId.Should().NotBeEmpty();
        @event.TransactionId.Should().Be(transactionId);
        @event.AccountId.Should().Be(accountId);
        @event.Amount.Should().Be(amount);
        @event.TransactionDate.Should().Be(transactionDate);
        @event.OccurredOn.Should().BeOnOrAfter(before);
        @event.OccurredOn.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void FraudDetectedEvent_ShouldInitializeWithCorrectValues()
    {
        var transactionId = Guid.NewGuid();
        var accountId = "ACC123";
        var riskLevel = FraudRiskLevel.High;
        var fraudScore = 85.5;
        var before = DateTime.UtcNow;

        var @event = new FraudDetectedEvent(transactionId, accountId, riskLevel, fraudScore);
        var after = DateTime.UtcNow;

        @event.EventId.Should().NotBeEmpty();
        @event.TransactionId.Should().Be(transactionId);
        @event.AccountId.Should().Be(accountId);
        @event.RiskLevel.Should().Be(riskLevel);
        @event.FraudScore.Should().Be(fraudScore);
        @event.OccurredOn.Should().BeOnOrAfter(before);
        @event.OccurredOn.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void TransactionStatusChangedEvent_WithReason_ShouldInitializeCorrectly()
    {
        var transactionId = Guid.NewGuid();
        var newStatus = TransactionStatus.Declined;
        var reason = "Fraud detected";

        var @event = new TransactionStatusChangedEvent(transactionId, newStatus, reason);

        @event.TransactionId.Should().Be(transactionId);
        @event.NewStatus.Should().Be(newStatus);
        @event.Reason.Should().Be(reason);
    }

    [Fact]
    public void RuleTriggeredEvent_ShouldInitializeWithCorrectValues()
    {
        var ruleId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var ruleName = "High Value Transaction Rule";
        var riskLevel = FraudRiskLevel.High;
        var score = 85.0;
        var before = DateTime.UtcNow;

        var @event = new RuleTriggeredEvent(ruleId, transactionId, ruleName, riskLevel, score);
        var after = DateTime.UtcNow;

        @event.EventId.Should().NotBeEmpty();
        @event.RuleId.Should().Be(ruleId);
        @event.TransactionId.Should().Be(transactionId);
        @event.RuleName.Should().Be(ruleName);
        @event.RiskLevel.Should().Be(riskLevel);
        @event.Score.Should().Be(score);
        @event.OccurredOn.Should().BeOnOrAfter(before);
        @event.OccurredOn.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void AllDomainEvents_ShouldHaveUniqueEventIds()
    {
        var events = new List<IDomainEvent>
        {
            new TransactionCreatedEvent(Guid.NewGuid(), "ACC123", new Money(100, "USD"), DateTime.UtcNow),
            new FraudDetectedEvent(Guid.NewGuid(), "ACC123", FraudRiskLevel.High, 85.0),
            new TransactionStatusChangedEvent(Guid.NewGuid(), TransactionStatus.Approved),
            new RuleTriggeredEvent(Guid.NewGuid(), Guid.NewGuid(), "Rule", FraudRiskLevel.Medium, 60.0)
        };

        var eventIds = events.Select(e => e.EventId).ToList();
        eventIds.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void AllDomainEvents_ShouldHaveOccurredOnTimestamp()
    {
        var before = DateTime.UtcNow;

        var events = new List<IDomainEvent>
        {
            new TransactionCreatedEvent(Guid.NewGuid(), "ACC123", new Money(100, "USD"), DateTime.UtcNow),
            new FraudDetectedEvent(Guid.NewGuid(), "ACC123", FraudRiskLevel.High, 85.0),
            new TransactionStatusChangedEvent(Guid.NewGuid(), TransactionStatus.Approved),
            new RuleTriggeredEvent(Guid.NewGuid(), Guid.NewGuid(), "Rule", FraudRiskLevel.Medium, 60.0)
        };

        var after = DateTime.UtcNow;

        events.Should().AllSatisfy(e =>
        {
            e.OccurredOn.Should().BeOnOrAfter(before);
            e.OccurredOn.Should().BeOnOrBefore(after);
        });
    }
}
