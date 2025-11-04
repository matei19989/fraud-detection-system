using FluentAssertions;
using FraudDetection.Application.RequestHandlers.QueryHandlers;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using FraudDetection.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FraudDetection.Application.UnitTests.QueryHandlers;

public class GetRecentTransactionsQueryHandlerTests
{
    private TestApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldReturnRequestedNumberOfTransactions()
    {
        using var context = CreateContext();

        for (int i = 0; i < 10; i++)
        {
            var transaction = new Transaction(
                $"ACC{i}",
                new Money(100 * i, "USD"),
                TransactionType.Purchase,
                new MerchantInfo($"M{i}", $"Store {i}", "Retail"),
                new Location(40.7128, -74.0060, "US", "New York", "192.168.1.1"),
                $"DEV{i}",
                $"Transaction {i}"
            );
            await context.Transactions.AddAsync(transaction);
        }
        await context.SaveChangesAsync();

        var handler = new GetRecentTransactionsQueryHandler(context);
        var query = new GetRecentTransactionsQuery { Count = 5 };

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(5);
    }
}