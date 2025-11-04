using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Commands;

public record CreateTransactionCommand : IRequest<TransactionDto>
{
    public required string AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string Type { get; init; }
    public required string MerchantId { get; init; }
    public required string MerchantName { get; init; }
    public required string MerchantCategory { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? IpAddress { get; init; }
    public string? DeviceId { get; init; }
    public string? Description { get; init; }
}