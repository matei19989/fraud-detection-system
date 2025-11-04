using FluentAssertions;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.RequestHandlers.CommandHandlers;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FraudDetection.Application.UnitTests.CommandHandlers;

public class ActivateFraudRuleCommandHandlerTests
{
    private TestApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_WithExistingRule_ShouldActivateRule()
    {
        using var context = CreateContext();

        var rule = new FraudRule(
            "Test Rule",
            "Test description",
            FraudRiskLevel.Medium,
            "VelocityCheck",
            "{}",
            1
        );
        rule.Deactivate();
        await context.FraudRules.AddAsync(rule);
        await context.SaveChangesAsync();

        var handler = new ActivateFraudRuleCommandHandler(context);
        var command = new ActivateFraudRuleCommand { RuleId = rule.Id };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        var updatedRule = await context.FraudRules.FirstOrDefaultAsync(r => r.Id == rule.Id);
        updatedRule!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentRule_ShouldReturnFalse()
    {
        using var context = CreateContext();
        var handler = new ActivateFraudRuleCommandHandler(context);
        var command = new ActivateFraudRuleCommand { RuleId = Guid.NewGuid() };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}