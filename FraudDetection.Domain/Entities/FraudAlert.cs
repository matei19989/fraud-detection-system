using FraudDetection.Domain.Common;
using FraudDetection.Domain.Enums;

namespace FraudDetection.Domain.Entities;

public class FraudAlert : BaseEntity
{
    public Guid TransactionId { get; private set; }
    public Guid? RuleId { get; private set; }
    public string RuleName { get; private set; }
    public AlertStatus Status { get; private set; }
    public FraudRiskLevel RiskLevel { get; private set; }
    public double Score { get; private set; }
    public string Message { get; private set; }
    public string? Details { get; private set; }
    public string? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewNotes { get; private set; }

    public Transaction Transaction { get; private set; } = null!;
    public FraudRule? Rule { get; private set; }

#pragma warning disable CS8618
    private FraudAlert() { } // For EF Core
#pragma warning restore CS8618

    public FraudAlert(
        Guid transactionId,
        string ruleName,
        FraudRiskLevel riskLevel,
        double score,
        string message,
        string? details = null,
        Guid? ruleId = null)
    {
        if (string.IsNullOrWhiteSpace(ruleName))
            throw new ArgumentException("Rule name cannot be null or empty", nameof(ruleName));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or empty", nameof(message));

        if (score < 0 || score > 100)
            throw new ArgumentException("Score must be between 0 and 100", nameof(score));

        TransactionId = transactionId;
        RuleId = ruleId;
        RuleName = ruleName;
        RiskLevel = riskLevel;
        Score = score;
        Message = message;
        Details = details;
        Status = AlertStatus.New;
    }

    public void Investigate(string investigatedBy)
    {
        if (Status != AlertStatus.New)
            throw new InvalidOperationException($"Cannot investigate alert in {Status} status");

        Status = AlertStatus.Investigating;
        ReviewedBy = investigatedBy;
        ReviewedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Resolve(string resolvedBy, string notes)
    {
        if (Status != AlertStatus.Investigating)
            throw new InvalidOperationException($"Cannot resolve alert in {Status} status");

        Status = AlertStatus.Resolved;
        ReviewedBy = resolvedBy;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes;
        SetUpdatedAt();
    }

    public void MarkAsFalsePositive(string reviewedBy, string notes)
    {
        if (Status == AlertStatus.ConfirmedFraud)
            throw new InvalidOperationException("Cannot mark confirmed fraud as false positive");

        Status = AlertStatus.FalsePositive;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes;
        SetUpdatedAt();
    }

    public void ConfirmFraud(string confirmedBy, string notes)
    {
        Status = AlertStatus.ConfirmedFraud;
        ReviewedBy = confirmedBy;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes;
        SetUpdatedAt();
    }
}
