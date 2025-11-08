using FluentAssertions;
using FraudDetection.Application.RequestHandlers.CommandHandlers;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace FraudDetection.Application.UnitTests.CommandHandlers;

public class UpdateFraudRuleCommandHandlerTests
{
    private TestApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateRule()
    {
        using var context = CreateContext();

        var rule = new FraudRule(
            "Original Rule",
            "Original description",
            FraudRiskLevel.Low,
            "VelocityCheck",
            "{}",
            1
        );
        await context.FraudRules.AddAsync(rule);
        await context.SaveChangesAsync();
        
        var mockCache = new Mock<IMemoryCache>();
        var handler = new UpdateFraudRuleCommandHandler(context, mockCache.Object);
        var command = new UpdateFraudRuleCommand
        {
            RuleId = rule.Id,
            Name = "Updated Rule",
            Description = "Updated description",
            RiskLevel = "High"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Rule");
        result.Description.Should().Be("Updated description");
        result.RiskLevel.Should().Be("High");
    }

    [Fact]
    public async Task Handle_WithNonExistentRule_ShouldReturnNull()
    {
        using var context = CreateContext();
        var mockCache = new Mock<IMemoryCache>();
        var handler = new UpdateFraudRuleCommandHandler(context, mockCache.Object);
        var command = new UpdateFraudRuleCommand
        {
            RuleId = Guid.NewGuid(),
            Name = "Updated Rule",
            Description = "Updated description",
            RiskLevel = "High"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}