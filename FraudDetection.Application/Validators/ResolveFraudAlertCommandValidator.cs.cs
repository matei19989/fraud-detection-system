using FluentValidation;
using FraudDetection.Application.Requests.Commands;

namespace FraudDetection.Application.Validators;

public class ResolveFraudAlertCommandValidator : AbstractValidator<ResolveFraudAlertCommand>
{
    public ResolveFraudAlertCommandValidator()
    {
        RuleFor(x => x.AlertId)
            .NotEmpty()
            .WithMessage("Alert ID is required");

        RuleFor(x => x.ResolvedBy)
            .NotEmpty()
            .WithMessage("Resolved by is required")
            .MaximumLength(200)
            .WithMessage("Resolved by must not exceed 200 characters")
            .EmailAddress()
            .WithMessage("Resolved by must be a valid email address");

        RuleFor(x => x.Notes)
            .NotEmpty()
            .WithMessage("Notes are required")
            .MaximumLength(2000)
            .WithMessage("Notes must not exceed 2000 characters");
    }
}