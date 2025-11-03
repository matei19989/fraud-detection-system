using FluentAssertions;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.RequestHandlers.CommandHandlers;
using FraudDetection.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FraudDetection.Application.UnitTests.CommandHandlers;

public class CreateFraudRuleCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockDbContext;
    private readonly Mock<DbSet<FraudRule>> _mockRuleSet;
    private readonly CreateFraudRuleCommandHandler _handler;

    public CreateFraudRuleCommandHandlerTests()
    {
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockRuleSet = new Mock<DbSet<FraudRule>>();
        _mockDbContext.Setup(x => x.FraudRules).Returns(_mockRuleSet.Object);
        _handler = new CreateFraudRuleCommandHandler(_mockDbContext.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateFraudRule()
    {
        var command = new CreateFraudRuleCommand
        {
            Name = "High Value Rule",
            Description = "Detects high value transactions",
            RiskLevel = "High",
            RuleType = "AmountThreshold",
            ConditionsJson = "{\"threshold\": 1000}",
            Priority = 5
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("High Value Rule");
        result.RiskLevel.Should().Be("High");
        result.IsActive.Should().BeTrue();
        result.TimesTriggered.Should().Be(0);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidRiskLevel_ShouldThrowArgumentException()
    {
        var command = new CreateFraudRuleCommand
        {
            Name = "Test Rule",
            Description = "Test",
            RiskLevel = "InvalidLevel",
            RuleType = "AmountThreshold",
            ConditionsJson = "{}",
            Priority = 1
        };

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Invalid risk level*");
    }
}