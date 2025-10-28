namespace FraudDetection.Application.DTOs;

public record AccountDto
{
    public Guid Id { get; init; }
    public string AccountId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public int TotalTransactions { get; init; }
    public decimal AverageTransactionAmount { get; init; }
    public bool IsSuspended { get; init; }
    public DateTime? LastTransactionDate { get; init; }
}