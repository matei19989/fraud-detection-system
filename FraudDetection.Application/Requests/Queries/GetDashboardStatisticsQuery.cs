using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Queries;

public record GetDashboardStatisticsQuery : IRequest<DashboardStatisticsDto>
{
}