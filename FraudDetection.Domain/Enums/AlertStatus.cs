namespace FraudDetection.Domain.Enums;

public enum AlertStatus
{
    New = 0,
    Investigating = 1,
    Resolved = 2,
    FalsePositive = 3,
    ConfirmedFraud = 4
}