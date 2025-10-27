using FraudDetection.Domain.Common;
using FraudDetection.Domain.ValueObjects;

namespace FraudDetection.Domain.Entities;

public class Account : BaseEntity
{
    public string AccountId { get; private set; }
    public string Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public int TotalTransactions { get; private set; }
    public decimal AverageTransactionAmount { get; private set; }
    public Money TotalSpent { get; private set; }
    public Location? LastKnownLocation { get; private set; }
    public DateTime? LastTransactionDate { get; private set; }
    public bool IsSuspended { get; private set; }
    public string? SuspensionReason { get; private set; }

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    #pragma warning disable CS8618
    private Account() { }
    #pragma warning restore CS8618

    public Account(
        string accountId,
        string email,
        string? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be null or empty", nameof(accountId));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        AccountId = accountId;
        Email = email;
        PhoneNumber = phoneNumber;
        RegistrationDate = DateTime.UtcNow;
        TotalTransactions = 0;
        AverageTransactionAmount = 0;
        TotalSpent = Money.Zero("USD");
        IsSuspended = false;
    }

    public void UpdateTransactionStatistics(Money transactionAmount)
    {
        TotalTransactions++;

        if (TotalSpent.Currency == transactionAmount.Currency)
        {
            TotalSpent = TotalSpent.Add(transactionAmount);
        }
        else
        {
            TotalSpent = transactionAmount;
        }

        AverageTransactionAmount = TotalSpent.Amount / TotalTransactions;
        LastTransactionDate = DateTime.UtcNow;

        SetUpdatedAt();
    }

    public void UpdateLastKnownLocation(Location location)
    {
        LastKnownLocation = location ?? throw new ArgumentNullException(nameof(location));
        SetUpdatedAt();
    }

    public void Suspend(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Suspension reason is required", nameof(reason));

        IsSuspended = true;
        SuspensionReason = reason;
        SetUpdatedAt();
    }

    public void Reactivate()
    {
        if (!IsSuspended)
            throw new InvalidOperationException("Account is not suspended");

        IsSuspended = false;
        SuspensionReason = null;
        SetUpdatedAt();
    }

    public void UpdateContactInfo(string email, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        Email = email;
        PhoneNumber = phoneNumber;
        SetUpdatedAt();
    }

    public bool IsTransactionAmountUnusual(decimal amount)
    {
        if (TotalTransactions < 5)
            return false;

        //Potential unusual amount
        return amount > (AverageTransactionAmount * 5);
    }

    public bool HasRecentActivity(TimeSpan timeWindow)
    {
        if (!LastTransactionDate.HasValue)
            return false;

        return DateTime.UtcNow - LastTransactionDate.Value <= timeWindow;
    }
}