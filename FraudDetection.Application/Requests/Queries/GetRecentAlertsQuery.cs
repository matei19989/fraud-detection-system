using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Queries;

public record GetRecentAlertsQuery : IRequest<List<FraudAlertDto>>
{
    public int Count { get; init; } = 10;
}