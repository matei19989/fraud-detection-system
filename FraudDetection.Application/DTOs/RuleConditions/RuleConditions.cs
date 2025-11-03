namespace FraudDetection.Application.DTOs.RuleConditions;

public abstract class RuleCondition
{
    public string Type { get; set; } = string.Empty;
}

public class AmountThresholdCondition : RuleCondition
{
    public decimal Threshold { get; set; }
    public string Operator { get; set; } = "GreaterThan";

    public AmountThresholdCondition()
    {
        Type = "AmountThreshold";
    }
}

public class VelocityCondition : RuleCondition
{
    public int TransactionCount { get; set; }
    public int TimeWindowMinutes { get; set; }

    public VelocityCondition()
    {
        Type = "Velocity";
    }
}

public class LocationAnomalyCondition : RuleCondition
{
    public double MaxDistanceKm { get; set; }
    public int TimeWindowMinutes { get; set; }

    public LocationAnomalyCondition()
    {
        Type = "LocationAnomaly";
    }
}

public class NewAccountCondition : RuleCondition
{
    public int AccountAgeDays { get; set; }
    public decimal? MaxTransactionAmount { get; set; }

    public NewAccountCondition()
    {
        Type = "NewAccount";
    }
}

public class UnusualMerchantCondition : RuleCondition
{
    public List<string> HighRiskCategories { get; set; } = new();

    public UnusualMerchantCondition()
    {
        Type = "UnusualMerchant";
    }
}

public class TimeOfDayCondition : RuleCondition
{
    public int StartHour { get; set; }
    public int EndHour { get; set; }

    public TimeOfDayCondition()
    {
        Type = "TimeOfDay";
    }
}

public class AmountDeviationCondition : RuleCondition
{
    public double StandardDeviationMultiplier { get; set; } = 3.0;
    public int MinimumTransactionCount { get; set; } = 5;

    public AmountDeviationCondition()
    {
        Type = "AmountDeviation";
    }
}

public class CompositeCondition : RuleCondition
{
    public string Logic { get; set; } = "AND"; // AND, OR
    public List<RuleCondition> Conditions { get; set; } = new();

    public CompositeCondition()
    {
        Type = "Composite";
    }
}