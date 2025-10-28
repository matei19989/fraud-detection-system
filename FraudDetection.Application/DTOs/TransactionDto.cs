namespace FraudDetection.Application.DTOs;

public record TransactionDto
{
    public Guid Id { get; init; }
    public string AccountId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string MerchantId { get; init; } = string.Empty;
    public string MerchantName { get; init; } = string.Empty;
    public string MerchantCategory { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public double FraudScore { get; init; }
    public DateTime TransactionDate { get; init; }
    public string? DeviceId { get; init; }
    public DateTime CreatedAt { get; init; }
}