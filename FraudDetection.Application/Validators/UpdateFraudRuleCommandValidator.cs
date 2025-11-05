using FluentValidation;
using FraudDetection.Application.Requests.Commands;

namespace FraudDetection.Application.Validators;

public class UpdateFraudRuleCommandValidator : AbstractValidator<UpdateFraudRuleCommand>
{
    public UpdateFraudRuleCommandValidator()
    {
        RuleFor(x => x.RuleId)
            .NotEmpty()
            .WithMessage("Rule ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Rule name is required")
            .MaximumLength(200)
            .WithMessage("Rule name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Rule description is required")
            .MaximumLength(1000)
            .WithMessage("Rule description must not exceed 1000 characters");

        RuleFor(x => x.RiskLevel)
            .NotEmpty()
            .WithMessage("Risk level is required")
            .Must(BeValidRiskLevel)
            .WithMessage("Risk level must be one of: None, Low, Medium, High, Critical");
    }

    private static bool BeValidRiskLevel(string riskLevel)
    {
        var validRiskLevels = new[] { "None", "Low", "Medium", "High", "Critical" };
        return validRiskLevels.Contains(riskLevel, StringComparer.OrdinalIgnoreCase);
    }
}