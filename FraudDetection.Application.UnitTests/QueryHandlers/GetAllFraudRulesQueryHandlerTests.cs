using FluentAssertions;
using FraudDetection.Application.RequestHandlers.QueryHandlers;
using FraudDetection.Application.Requests.Queries;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FraudDetection.Application.UnitTests.QueryHandlers;

public class GetAllFraudRulesQueryHandlerTests
{
    private TestApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_WithoutFilters_ShouldReturnAllRules()
    {
        using var context = CreateContext();

        await context.FraudRules.AddAsync(new FraudRule("Rule1", "Desc1", FraudRiskLevel.High, "VelocityCheck", "{}", 1));
        await context.FraudRules.AddAsync(new FraudRule("Rule2", "Desc2", FraudRiskLevel.Low, "AmountCheck", "{}", 2));
        await context.SaveChangesAsync();

        var handler = new GetAllFraudRulesQueryHandler(context);
        var query = new GetAllFraudRulesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithIsActiveFilter_ShouldReturnFilteredRules()
    {
        using var context = CreateContext();

        var activeRule = new FraudRule("Active", "Desc", FraudRiskLevel.High, "VelocityCheck", "{}", 1);
        var inactiveRule = new FraudRule("Inactive", "Desc", FraudRiskLevel.Low, "AmountCheck", "{}", 2);
        inactiveRule.Deactivate();

        await context.FraudRules.AddAsync(activeRule);
        await context.FraudRules.AddAsync(inactiveRule);
        await context.SaveChangesAsync();

        var handler = new GetAllFraudRulesQueryHandler(context);
        var query = new GetAllFraudRulesQuery { IsActive = true };

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Active");
    }

    [Fact]
    public async Task Handle_WithRuleTypeFilter_ShouldReturnFilteredRules()
    {
        using var context = CreateContext();

        await context.FraudRules.AddAsync(new FraudRule("Rule1", "Desc", FraudRiskLevel.High, "VelocityCheck", "{}", 1));
        await context.FraudRules.AddAsync(new FraudRule("Rule2", "Desc", FraudRiskLevel.Low, "AmountCheck", "{}", 2));
        await context.SaveChangesAsync();

        var handler = new GetAllFraudRulesQueryHandler(context);
        var query = new GetAllFraudRulesQuery { RuleType = "VelocityCheck" };

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().RuleType.Should().Be("VelocityCheck");
    }
}