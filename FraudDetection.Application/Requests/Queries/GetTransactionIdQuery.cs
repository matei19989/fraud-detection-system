using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Queries;

public record GetTransactionByIdQuery : IRequest<TransactionDto?>
{
    public required Guid TransactionId { get; init; }
}