namespace FraudDetection.Application.DTOs;

public record FraudRuleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public int Priority { get; init; }
    public string RuleType { get; init; } = string.Empty;
    public int TimesTriggered { get; init; }
    public DateTime? LastTriggeredAt { get; init; }
}