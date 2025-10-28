using MediatR;

namespace FraudDetection.Application.Requests.Commands;

public record MarkAsFalsePositiveCommand : IRequest<bool>
{
    public required Guid AlertId { get; init; }
    public required string ReviewedBy { get; init; }
    public required string Notes { get; init; }
}