using FluentAssertions;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using Xunit;

namespace FraudDetection.Domain.UnitTests.Entities;

public class FraudAlertTests
{
    private readonly Guid _transactionId = Guid.NewGuid();
    private readonly Guid _ruleId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateFraudAlert()
    {
        var alert = new FraudAlert(
            _transactionId,
            "High Value Rule",
            FraudRiskLevel.High,
            85.5,
            "Transaction exceeds threshold",
            "Additional details here",
            _ruleId);

        alert.TransactionId.Should().Be(_transactionId);
        alert.RuleId.Should().Be(_ruleId);
        alert.RuleName.Should().Be("High Value Rule");
        alert.RiskLevel.Should().Be(FraudRiskLevel.High);
        alert.Score.Should().Be(85.5);
        alert.Message.Should().Be("Transaction exceeds threshold");
        alert.Details.Should().Be("Additional details here");
        alert.Status.Should().Be(AlertStatus.New);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_WithInvalidScore_ShouldThrowArgumentException(double score)
    {
        Action act = () => new FraudAlert(_transactionId, "Test Rule", FraudRiskLevel.High, score, "Message");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Score must be between 0 and 100*");
    }

    [Fact]
    public void Constructor_ShouldSetStatusToNew()
    {
        var alert = new FraudAlert(_transactionId, "Test Rule", FraudRiskLevel.High, 80, "Test message");

        alert.Status.Should().Be(AlertStatus.New);
    }

    [Fact]
    public void Investigate_FromNewStatus_ShouldChangeStatusToInvestigating()
    {
        var alert = new FraudAlert(_transactionId, "Test Rule", FraudRiskLevel.High, 80, "Message");
        var investigator = "john.doe@example.com";
        var before = DateTime.UtcNow;

        alert.Investigate(investigator);
        var after = DateTime.UtcNow;

        alert.Status.Should().Be(AlertStatus.Investigating);
        alert.ReviewedBy.Should().Be(investigator);
        alert.ReviewedAt.Should().NotBeNull();
        alert.ReviewedAt.Should().BeOnOrAfter(before);
        alert.ReviewedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void Investigate_FromNonNewStatus_ShouldThrowInvalidOperationException()
    {
        var alert = new FraudAlert(_transactionId, "Test Rule", FraudRiskLevel.High, 80, "Message");
        alert.Investigate("investigator");

        Action act = () => alert.Investigate("investigator2");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot investigate alert in*");
    }

    [Fact]
    public void Resolve_FromInvestigatingStatus_ShouldChangeStatusToResolved()
    {
        var alert = new FraudAlert(_transactionId, "Test Rule", FraudRiskLevel.High, 80, "Message");
        alert.Investigate("investigator");
        var resolver = "resolver@example.com";
        var notes = "Issue resolved successfully";

        alert.Resolve(resolver, notes);

        alert.Status.Should().Be(AlertStatus.Resolved);
        alert.ReviewedBy.Should().Be(resolver);
        alert.ReviewNotes.Should().Be(notes);
    }

    [Fact]
    public void MarkAsFalsePositive_FromNewStatus_ShouldChangeStatusToFalsePositive()
    {
        var alert = new FraudAlert(_transactionId, "Test Rule", FraudRiskLevel.High, 80, "Message");
        var reviewer = "reviewer@example.com";
        var notes = "False alarm";

        alert.MarkAsFalsePositive(reviewer, notes);

        alert.Status.Should().Be(AlertStatus.FalsePositive);
        alert.ReviewedBy.Should().Be(reviewer);
        alert.ReviewNotes.Should().Be(notes);
    }

    [Fact]
    public void MarkAsFalsePositive_FromConfirmedFraudStatus_ShouldThrowInvalidOperationException()
    {
        var alert = new FraudAlert(_transactionId, "Test Rule", FraudRiskLevel.High, 80, "Message");
        alert.ConfirmFraud("confirmer", "confirmed");

        Action act = () => alert.MarkAsFalsePositive("reviewer", "notes");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot mark confirmed fraud as false positive*");
    }

    [Fact]
    public void ConfirmFraud_FromAnyStatus_ShouldChangeStatusToConfirmedFraud()
    {
        var alert = new FraudAlert(_transactionId, "Test Rule", FraudRiskLevel.High, 80, "Message");
        var confirmer = "confirmer@example.com";
        var notes = "Fraud confirmed";

        alert.ConfirmFraud(confirmer, notes);

        alert.Status.Should().Be(AlertStatus.ConfirmedFraud);
        alert.ReviewedBy.Should().Be(confirmer);
        alert.ReviewNotes.Should().Be(notes);
    }

    [Fact]
    public void FraudAlert_CompleteWorkflow_ShouldTransitionThroughStatuses()
    {
        var alert = new FraudAlert(_transactionId, "Test Rule", FraudRiskLevel.High, 80, "Suspicious transaction");

        alert.Status.Should().Be(AlertStatus.New);

        alert.Investigate("investigator@example.com");
        alert.Status.Should().Be(AlertStatus.Investigating);
        alert.ReviewedBy.Should().Be("investigator@example.com");

        alert.Resolve("resolver@example.com", "Resolved after investigation");
        alert.Status.Should().Be(AlertStatus.Resolved);
        alert.ReviewedBy.Should().Be("resolver@example.com");
        alert.ReviewNotes.Should().Be("Resolved after investigation");
    }
}
