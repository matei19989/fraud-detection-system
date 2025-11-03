using FluentAssertions;
using FraudDetection.Application.RequestHandlers.QueryHandlers;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using FraudDetection.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FraudDetection.Application.UnitTests.QueryHandlers;

public class GetTransactionByIdQueryHandlerTests
{
    private TestApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_WithExistingTransaction_ShouldReturnTransactionDto()
    {
        using var context = CreateContext();
        var transaction = new Transaction(
            "ACC123",
            new Money(500, "USD"),
            TransactionType.Purchase,
            new MerchantInfo("M123", "Test Store", "Retail"),
            new Location(40.7128, -74.0060, "US", "New York", "192.168.1.1"),
            "DEV123",
            "Test transaction"
        );

        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();

        var handler = new GetTransactionByIdQueryHandler(context);
        var query = new GetTransactionByIdQuery { TransactionId = transaction.Id };

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(transaction.Id);
        result.AccountId.Should().Be("ACC123");
    }

    [Fact]
    public async Task Handle_WithNonExistentTransaction_ShouldReturnNull()
    {
        using var context = CreateContext();
        var handler = new GetTransactionByIdQueryHandler(context);
        var query = new GetTransactionByIdQuery { TransactionId = Guid.NewGuid() };

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}