using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Queries;

public record GetFraudAlertByIdQuery : IRequest<FraudAlertDto?>
{
    public required Guid AlertId { get; init; }
}