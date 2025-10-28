using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Queries;

public record GetRecentTransactionsQuery : IRequest<List<TransactionDto>>
{
    public int Count { get; init; } = 10;
}