using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Queries;

public record GetTransactionsByAccountQuery : IRequest<List<TransactionDto>>
{
    public required string AccountId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}