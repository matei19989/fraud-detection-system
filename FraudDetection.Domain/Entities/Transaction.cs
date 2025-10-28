using FraudDetection.Domain.Common;
using FraudDetection.Domain.Enums;
using FraudDetection.Domain.Events;
using FraudDetection.Domain.ValueObjects;

namespace FraudDetection.Domain.Entities;

public class Transaction : BaseEntity
{
    public string AccountId { get; private set; }
    public Money Amount { get; private set; }

    public Account? Account { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public MerchantInfo Merchant { get; private set; }
    public Location Location { get; private set; }
    public FraudRiskLevel RiskLevel { get; private set; }
    public double FraudScore { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string? DeviceId { get; private set; }
    public string? Description { get; private set; }

    // Navigation property for fraud alerts
    private readonly List<FraudAlert> _fraudAlerts = new();
    public IReadOnlyCollection<FraudAlert> FraudAlerts => _fraudAlerts.AsReadOnly();

#pragma warning disable CS8618
    private Transaction() { }
#pragma warning restore CS8618

    public Transaction(
        string accountId,
        Money amount,
        TransactionType type,
        MerchantInfo merchant,
        Location location,
        string? deviceId = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be null or empty", nameof(accountId));

        AccountId = accountId;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Type = type;
        Merchant = merchant ?? throw new ArgumentNullException(nameof(merchant));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        DeviceId = deviceId;
        Description = description;

        Status = TransactionStatus.Pending;
        RiskLevel = FraudRiskLevel.None;
        FraudScore = 0;
        TransactionDate = DateTime.UtcNow;

        // Raise domain event
        AddDomainEvent(new TransactionCreatedEvent(Id, AccountId, Amount, TransactionDate));
    }

    public void UpdateFraudScore(double score, FraudRiskLevel riskLevel)
    {
        if (score < 0 || score > 100)
            throw new ArgumentException("Fraud score must be between 0 and 100", nameof(score));

        var previousScore = FraudScore;
        var previousRiskLevel = RiskLevel;

        FraudScore = score;
        RiskLevel = riskLevel;
        SetUpdatedAt();

        if (riskLevel >= FraudRiskLevel.High && previousRiskLevel < FraudRiskLevel.High)
        {
            AddDomainEvent(new FraudDetectedEvent(Id, AccountId, riskLevel, score));
        }
    }

    public void Approve()
    {
        if (Status != TransactionStatus.Pending && Status != TransactionStatus.UnderReview)
            throw new InvalidOperationException($"Cannot approve transaction in {Status} status");

        Status = TransactionStatus.Approved;
        SetUpdatedAt();

        AddDomainEvent(new TransactionStatusChangedEvent(Id, Status));
    }

    public void Decline(string reason)
    {
        if (Status == TransactionStatus.Approved)
            throw new InvalidOperationException("Cannot decline an approved transaction");

        Status = TransactionStatus.Declined;
        SetUpdatedAt();

        AddDomainEvent(new TransactionStatusChangedEvent(Id, Status, reason));
    }

    public void MarkForReview()
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException($"Cannot mark {Status} transaction for review");

        Status = TransactionStatus.UnderReview;
        SetUpdatedAt();

        AddDomainEvent(new TransactionStatusChangedEvent(Id, Status));
    }

    public void Cancel()
    {
        if (Status == TransactionStatus.Approved || Status == TransactionStatus.Declined)
            throw new InvalidOperationException($"Cannot cancel transaction in {Status} status");

        Status = TransactionStatus.Cancelled;
        SetUpdatedAt();

        AddDomainEvent(new TransactionStatusChangedEvent(Id, Status));
    }

    public void AddFraudAlert(FraudAlert alert)
    {
        ArgumentNullException.ThrowIfNull(alert);

        _fraudAlerts.Add(alert);
        SetUpdatedAt();
    }
}