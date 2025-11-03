using FluentAssertions;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using FraudDetection.Domain.Events;
using FraudDetection.Domain.ValueObjects;
using Xunit;

namespace FraudDetection.Domain.UnitTests.Entities;

public class TransactionTests
{
    private readonly Money _validAmount = new(100.50m, "USD");
    private readonly MerchantInfo _validMerchant = new("MERCH123", "Amazon", "E-commerce");
    private readonly Location _validLocation = new(40.7128, -74.0060, "USA", "New York");

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateTransaction()
    {
        var transaction = new Transaction(
            "ACC123",
            _validAmount,
            TransactionType.Purchase,
            _validMerchant,
            _validLocation,
            "DEVICE123",
            "Test purchase");

        transaction.AccountId.Should().Be("ACC123");
        transaction.Amount.Should().Be(_validAmount);
        transaction.Type.Should().Be(TransactionType.Purchase);
        transaction.Merchant.Should().Be(_validMerchant);
        transaction.Location.Should().Be(_validLocation);
        transaction.DeviceId.Should().Be("DEVICE123");
        transaction.Description.Should().Be("Test purchase");
        transaction.Status.Should().Be(TransactionStatus.Pending);
        transaction.RiskLevel.Should().Be(FraudRiskLevel.None);
        transaction.FraudScore.Should().Be(0);
    }

    [Fact]
    public void Constructor_ShouldRaiseTransactionCreatedEvent()
    {
        var transaction = new Transaction("ACC123", _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);

        transaction.DomainEvents.Should().ContainSingle();
        var domainEvent = transaction.DomainEvents.First();
        domainEvent.Should().BeOfType<TransactionCreatedEvent>();

        var transactionCreatedEvent = (TransactionCreatedEvent)domainEvent;
        transactionCreatedEvent.TransactionId.Should().Be(transaction.Id);
        transactionCreatedEvent.AccountId.Should().Be("ACC123");
        transactionCreatedEvent.Amount.Should().Be(_validAmount);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyAccountId_ShouldThrowArgumentException(string? accountId)
    {
        Action act = () => new Transaction(accountId!, _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Account ID cannot be null or empty*");
    }

    [Fact]
    public void UpdateFraudScore_WithValidScore_ShouldUpdateScoreAndRiskLevel()
    {
        var transaction = new Transaction("ACC123", _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);
        transaction.ClearDomainEvents();

        transaction.UpdateFraudScore(75.5, FraudRiskLevel.High);

        transaction.FraudScore.Should().Be(75.5);
        transaction.RiskLevel.Should().Be(FraudRiskLevel.High);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void UpdateFraudScore_WithInvalidScore_ShouldThrowArgumentException(double score)
    {
        var transaction = new Transaction("ACC123", _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);

        Action act = () => transaction.UpdateFraudScore(score, FraudRiskLevel.High);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Fraud score must be between 0 and 100*");
    }

    [Theory]
    [InlineData(FraudRiskLevel.High)]
    [InlineData(FraudRiskLevel.Critical)]
    public void UpdateFraudScore_ToHighOrCritical_ShouldRaiseFraudDetectedEvent(FraudRiskLevel riskLevel)
    {
        var transaction = new Transaction("ACC123", _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);
        transaction.ClearDomainEvents();

        transaction.UpdateFraudScore(80, riskLevel);

        transaction.DomainEvents.Should().ContainSingle();
        var domainEvent = transaction.DomainEvents.First();
        domainEvent.Should().BeOfType<FraudDetectedEvent>();

        var fraudEvent = (FraudDetectedEvent)domainEvent;
        fraudEvent.TransactionId.Should().Be(transaction.Id);
        fraudEvent.RiskLevel.Should().Be(riskLevel);
        fraudEvent.FraudScore.Should().Be(80);
    }

    [Fact]
    public void Approve_FromPendingStatus_ShouldChangeStatusToApproved()
    {
        var transaction = new Transaction("ACC123", _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);
        transaction.ClearDomainEvents();

        transaction.Approve();

        transaction.Status.Should().Be(TransactionStatus.Approved);
        transaction.DomainEvents.Should().ContainSingle();
        transaction.DomainEvents.First().Should().BeOfType<TransactionStatusChangedEvent>();
    }

    [Fact]
    public void Approve_FromDeclinedStatus_ShouldThrowInvalidOperationException()
    {
        var transaction = new Transaction("ACC123", _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);
        transaction.Decline("Fraud detected");

        Action act = () => transaction.Approve();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot approve transaction in*");
    }

    [Fact]
    public void Decline_FromPendingStatus_ShouldChangeStatusToDeclined()
    {
        var transaction = new Transaction("ACC123", _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);
        transaction.ClearDomainEvents();

        transaction.Decline("Suspicious activity");

        transaction.Status.Should().Be(TransactionStatus.Declined);
        transaction.DomainEvents.Should().ContainSingle();

        var statusEvent = (TransactionStatusChangedEvent)transaction.DomainEvents.First();
        statusEvent.Reason.Should().Be("Suspicious activity");
    }

    [Fact]
    public void MarkForReview_FromPendingStatus_ShouldChangeStatusToUnderReview()
    {
        var transaction = new Transaction("ACC123", _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);
        transaction.ClearDomainEvents();

        transaction.MarkForReview();

        transaction.Status.Should().Be(TransactionStatus.UnderReview);
        transaction.DomainEvents.Should().ContainSingle();
    }

    [Fact]
    public void Cancel_FromPendingStatus_ShouldChangeStatusToCancelled()
    {
        var transaction = new Transaction("ACC123", _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);
        transaction.ClearDomainEvents();

        transaction.Cancel();

        transaction.Status.Should().Be(TransactionStatus.Cancelled);
        transaction.DomainEvents.Should().ContainSingle();
    }

    [Fact]
    public void AddFraudAlert_WithValidAlert_ShouldAddToCollection()
    {
        var transaction = new Transaction("ACC123", _validAmount, TransactionType.Purchase, _validMerchant, _validLocation);
        var alert = new FraudAlert(transaction.Id, "Test Rule", FraudRiskLevel.High, 80, "Suspicious pattern");

        transaction.AddFraudAlert(alert);

        transaction.FraudAlerts.Should().ContainSingle();
        transaction.FraudAlerts.First().Should().Be(alert);
    }
}