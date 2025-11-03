using FluentAssertions;
using FraudDetection.Domain.ValueObjects;
using Xunit;

namespace FraudDetection.Domain.UnitTests.ValueObjects;

public class MerchantInfoTests
{
    [Fact]
    public void Constructor_WithValidValues_ShouldCreateMerchantInfo()
    {
        var merchant = new MerchantInfo("MERCH123", "Amazon", "E-commerce");

        merchant.MerchantId.Should().Be("MERCH123");
        merchant.MerchantName.Should().Be("Amazon");
        merchant.Category.Should().Be("E-commerce");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyMerchantId_ShouldThrowArgumentException(string? merchantId)
    {
        Action act = () => new MerchantInfo(merchantId!, "Amazon", "E-commerce");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Merchant ID cannot be null or empty*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyMerchantName_ShouldThrowArgumentException(string? merchantName)
    {
        Action act = () => new MerchantInfo("MERCH123", merchantName!, "E-commerce");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Merchant name cannot be null or empty*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyCategory_ShouldThrowArgumentException(string? category)
    {
        Action act = () => new MerchantInfo("MERCH123", "Amazon", category!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Category cannot be null or empty*");
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var merchant1 = new MerchantInfo("MERCH123", "Amazon", "E-commerce");
        var merchant2 = new MerchantInfo("MERCH123", "Amazon", "E-commerce");

        merchant1.Should().Be(merchant2);
        (merchant1 == merchant2).Should().BeTrue();
    }
}