using MediatR;
using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Requests.Commands;

public record CreateAccountCommand : IRequest<AccountDto>
{
    public required string AccountId { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
}