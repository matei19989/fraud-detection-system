using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Queries;

public record GetAllTransactionsQuery : IRequest<List<TransactionDto>>
{
    public string? Status { get; init; }
    public string? RiskLevel { get; init; }
    public string? AccountId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}