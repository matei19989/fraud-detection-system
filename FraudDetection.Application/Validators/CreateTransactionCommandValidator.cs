using FluentValidation;
using FraudDetection.Application.Requests.Commands;

namespace FraudDetection.Application.Validators;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a 3-letter ISO code");

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(type => new[] { "Purchase", "Refund", "Withdrawal", "Transfer", "Payment" }.Contains(type))
            .WithMessage("Invalid transaction type");

        RuleFor(x => x.MerchantId)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.MerchantName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.MerchantCategory)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180");
    }
}