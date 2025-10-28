using FraudDetection.Domain.Common;
using FraudDetection.Domain.Enums;

namespace FraudDetection.Domain.Entities;

public class FraudRule : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public FraudRiskLevel RiskLevel { get; private set; }
    public int Priority { get; private set; }
    public string RuleType { get; private set; }
    public string ConditionsJson { get; private set; }
    public int TimesTriggered { get; private set; }
    public DateTime? LastTriggeredAt { get; private set; }

    // Navigation property
    private readonly List<FraudAlert> _alerts = new();
    public IReadOnlyCollection<FraudAlert> Alerts => _alerts.AsReadOnly();

#pragma warning disable CS8618
    private FraudRule() { }
#pragma warning restore CS8618

    public FraudRule(
        string name,
        string description,
        FraudRiskLevel riskLevel,
        string ruleType,
        string conditionsJson,
        int priority = 1)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        if (string.IsNullOrWhiteSpace(ruleType))
            throw new ArgumentException("Rule type cannot be null or empty", nameof(ruleType));

        if (string.IsNullOrWhiteSpace(conditionsJson))
            throw new ArgumentException("Conditions JSON cannot be null or empty", nameof(conditionsJson));

        if (priority < 1)
            throw new ArgumentException("Priority must be at least 1", nameof(priority));

        Name = name;
        Description = description;
        RiskLevel = riskLevel;
        RuleType = ruleType;
        ConditionsJson = conditionsJson;
        Priority = priority;
        IsActive = true;
        TimesTriggered = 0;
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void UpdatePriority(int newPriority)
    {
        if (newPriority < 1)
            throw new ArgumentException("Priority must be at least 1", nameof(newPriority));

        Priority = newPriority;
        SetUpdatedAt();
    }

    public void UpdateConditions(string conditionsJson)
    {
        if (string.IsNullOrWhiteSpace(conditionsJson))
            throw new ArgumentException("Conditions JSON cannot be null or empty", nameof(conditionsJson));

        ConditionsJson = conditionsJson;
        SetUpdatedAt();
    }

    public void RecordTrigger()
    {
        TimesTriggered++;
        LastTriggeredAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void UpdateMetadata(string name, string description, FraudRiskLevel riskLevel)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        Name = name;
        Description = description;
        RiskLevel = riskLevel;
        SetUpdatedAt();
    }
}