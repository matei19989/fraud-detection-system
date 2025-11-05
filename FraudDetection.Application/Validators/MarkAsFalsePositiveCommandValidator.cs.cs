using FluentValidation;
using FraudDetection.Application.Requests.Commands;

namespace FraudDetection.Application.Validators;

public class MarkAsFalsePositiveCommandValidator : AbstractValidator<MarkAsFalsePositiveCommand>
{
    public MarkAsFalsePositiveCommandValidator()
    {
        RuleFor(x => x.AlertId)
            .NotEmpty()
            .WithMessage("Alert ID is required");

        RuleFor(x => x.ReviewedBy)
            .NotEmpty()
            .WithMessage("Reviewed by is required")
            .MaximumLength(200)
            .WithMessage("Reviewed by must not exceed 200 characters")
            .EmailAddress()
            .WithMessage("Reviewed by must be a valid email address");

        RuleFor(x => x.Notes)
            .NotEmpty()
            .WithMessage("Notes are required")
            .MaximumLength(2000)
            .WithMessage("Notes must not exceed 2000 characters");
    }
}