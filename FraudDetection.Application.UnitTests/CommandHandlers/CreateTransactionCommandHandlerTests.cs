using FluentAssertions;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.RequestHandlers.CommandHandlers;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FraudDetection.Application.UnitTests.CommandHandlers;

public class CreateTransactionCommandHandlerTests
{
    private readonly Mock<IFraudDetectionService> _mockFraudService;
    private readonly Mock<ILogger<CreateTransactionCommandHandler>> _mockLogger;

    public CreateTransactionCommandHandlerTests()
    {
        _mockFraudService = new Mock<IFraudDetectionService>();
        _mockLogger = new Mock<ILogger<CreateTransactionCommandHandler>>();
    }

    private TestApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateTransaction()
    {
        using var context = CreateContext();
        var handler = new CreateTransactionCommandHandler(context, _mockFraudService.Object, _mockLogger.Object);

        var command = new CreateTransactionCommand
        {
            AccountId = "ACC123",
            Amount = 500,
            Currency = "USD",
            Type = "Purchase",
            MerchantId = "M123",
            MerchantName = "Test Store",
            MerchantCategory = "Retail",
            Latitude = 40.7128,
            Longitude = -74.0060,
            Country = "US",
            City = "New York",
            IpAddress = "192.168.1.1",
            DeviceId = "DEV123",
            Description = "Test transaction"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccountId.Should().Be("ACC123");
        result.Amount.Should().Be(500);
        result.Currency.Should().Be("USD");

        var savedTransaction = await context.Transactions.FirstOrDefaultAsync();
        savedTransaction.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithExistingAccount_ShouldUpdateAccountStatistics()
    {
        using var context = CreateContext();

        var account = new Account(
            "ACC123",
            "john@example.com",
            "+1234567890"
        );
        await context.Accounts.AddAsync(account);
        await context.SaveChangesAsync();

        var handler = new CreateTransactionCommandHandler(context, _mockFraudService.Object, _mockLogger.Object);

        var command = new CreateTransactionCommand
        {
            AccountId = "ACC123",
            Amount = 500,
            Currency = "USD",
            Type = "Purchase",
            MerchantId = "M123",
            MerchantName = "Test Store",
            MerchantCategory = "Retail",
            Latitude = 40.7128,
            Longitude = -74.0060,
            Country = "US",
            City = "New York",
            IpAddress = "192.168.1.1"
        };

        await handler.Handle(command, CancellationToken.None);

        var updatedAccount = await context.Accounts.FirstOrDefaultAsync(a => a.AccountId == "ACC123");
        Assert.NotNull(updatedAccount);
        updatedAccount.TotalTransactions.Should().Be(1);
        updatedAccount.LastTransactionDate.Should().NotBeNull();
    }
}