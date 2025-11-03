using FluentAssertions;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.ValueObjects;
using Xunit;

namespace FraudDetection.Domain.UnitTests.Entities;

public class AccountTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateAccount()
    {
        var account = new Account("ACC123", "test@example.com", "+1234567890");

        account.AccountId.Should().Be("ACC123");
        account.Email.Should().Be("test@example.com");
        account.PhoneNumber.Should().Be("+1234567890");
        account.TotalTransactions.Should().Be(0);
        account.AverageTransactionAmount.Should().Be(0);
        account.IsSuspended.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyAccountId_ShouldThrowArgumentException(string? accountId)
    {
        Action act = () => new Account(accountId!, "test@example.com");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Account ID cannot be null or empty*");
    }

    [Fact]
    public void UpdateTransactionStatistics_ShouldCalculateAverageTransactionAmount()
    {
        var account = new Account("ACC123", "test@example.com");

        account.UpdateTransactionStatistics(new Money(100, "USD"));
        account.UpdateTransactionStatistics(new Money(200, "USD"));
        account.UpdateTransactionStatistics(new Money(150, "USD"));

        account.TotalTransactions.Should().Be(3);
        account.TotalSpent.Amount.Should().Be(450);
        account.AverageTransactionAmount.Should().Be(150);
    }

    [Fact]
    public void UpdateTransactionStatistics_WithSameCurrency_ShouldAddAmounts()
    {
        var account = new Account("ACC123", "test@example.com");

        account.UpdateTransactionStatistics(new Money(100, "USD"));
        account.UpdateTransactionStatistics(new Money(50, "USD"));

        account.TotalSpent.Amount.Should().Be(150);
    }

    [Fact]
    public void UpdateTransactionStatistics_WithDifferentCurrency_ShouldReplaceAmount()
    {
        var account = new Account("ACC123", "test@example.com");

        account.UpdateTransactionStatistics(new Money(100, "USD"));
        account.UpdateTransactionStatistics(new Money(50, "EUR"));

        account.TotalSpent.Amount.Should().Be(50);
        account.TotalSpent.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Suspend_WithValidReason_ShouldSuspendAccount()
    {
        var account = new Account("ACC123", "test@example.com");

        account.Suspend("Suspicious activity detected");

        account.IsSuspended.Should().BeTrue();
        account.SuspensionReason.Should().Be("Suspicious activity detected");
    }

    [Fact]
    public void Reactivate_WhenSuspended_ShouldReactivateAccount()
    {
        var account = new Account("ACC123", "test@example.com");
        account.Suspend("Test reason");

        account.Reactivate();

        account.IsSuspended.Should().BeFalse();
        account.SuspensionReason.Should().BeNull();
    }

    [Fact]
    public void Reactivate_WhenNotSuspended_ShouldThrowInvalidOperationException()
    {
        var account = new Account("ACC123", "test@example.com");

        Action act = () => account.Reactivate();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Account is not suspended*");
    }
}
