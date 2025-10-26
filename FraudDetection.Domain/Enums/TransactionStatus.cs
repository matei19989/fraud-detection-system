namespace FraudDetection.Domain.Enums;

public enum TransactionStatus
{
    Pending = 0,
    Approved = 1,
    Declined = 2,
    UnderReview = 3,
    Cancelled = 4
}