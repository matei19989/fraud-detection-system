namespace FraudDetection.Application.DTOs;

public record FraudAlertDto
{
    public Guid Id { get; init; }
    public Guid TransactionId { get; init; }
    public string RuleName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string RiskLevel { get; init; } = string.Empty;
    public double Score { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? Details { get; init; }
    public string? ReviewedBy { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}