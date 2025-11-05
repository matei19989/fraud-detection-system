using FluentValidation;
using FraudDetection.Application.Requests.Commands;

namespace FraudDetection.Application.Validators;

public class InvestigateFraudAlertCommandValidator : AbstractValidator<InvestigateFraudAlertCommand>
{
    public InvestigateFraudAlertCommandValidator()
    {
        RuleFor(x => x.AlertId)
            .NotEmpty()
            .WithMessage("Alert ID is required");

        RuleFor(x => x.InvestigatedBy)
            .NotEmpty()
            .WithMessage("Investigated by is required")
            .MaximumLength(200)
            .WithMessage("Investigated by must not exceed 200 characters")
            .EmailAddress()
            .WithMessage("Investigated by must be a valid email address");
    }
}