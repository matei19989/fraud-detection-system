using FluentAssertions;
using FraudDetection.Domain.ValueObjects;
using Xunit;

namespace FraudDetection.Domain.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidValues_ShouldCreateMoney()
    {
        var money = new Money(100.50m, "USD");

        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("USD");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Constructor_WithNegativeAmount_ShouldThrowArgumentException(decimal amount)
    {
        Action act = () => new Money(amount, "USD");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Amount cannot be negative*");
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDT")]
    [InlineData("$")]
    public void Constructor_WithInvalidCurrencyLength_ShouldThrowArgumentException(string currency)
    {
        Action act = () => new Money(100, currency);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Currency must be a 3-letter ISO code*");
    }

    [Fact]
    public void Zero_ShouldCreateMoneyWithZeroAmount()
    {
        var money = Money.Zero();

        money.Amount.Should().Be(0);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnCorrectSum()
    {
        var money1 = new Money(100, "USD");
        var money2 = new Money(50, "USD");

        var result = money1.Add(money2);

        result.Amount.Should().Be(150);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_WithDifferentCurrency_ShouldThrowInvalidOperationException()
    {
        var money1 = new Money(100, "USD");
        var money2 = new Money(50, "EUR");

        Action act = () => money1.Add(money2);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot add amounts with different currencies*");
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldReturnCorrectDifference()
    {
        var money1 = new Money(100, "USD");
        var money2 = new Money(30, "USD");

        var result = money1.Subtract(money2);

        result.Amount.Should().Be(70);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var money1 = new Money(100, "USD");
        var money2 = new Money(100, "USD");

        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }
}