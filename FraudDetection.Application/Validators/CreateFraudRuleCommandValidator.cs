using FluentValidation;
using FraudDetection.Application.Requests.Commands;
using System.Text.Json;

namespace FraudDetection.Application.Validators;

public class CreateFraudRuleCommandValidator : AbstractValidator<CreateFraudRuleCommand>
{
    public CreateFraudRuleCommandValidator()
    {
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

        RuleFor(x => x.RuleType)
            .NotEmpty()
            .WithMessage("Rule type is required")
            .MaximumLength(100)
            .WithMessage("Rule type must not exceed 100 characters");

        RuleFor(x => x.ConditionsJson)
            .NotEmpty()
            .WithMessage("Conditions JSON is required")
            .Must(BeValidJson)
            .WithMessage("Conditions must be valid JSON");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Priority must be at least 1");
    }

    private static bool BeValidRiskLevel(string riskLevel)
    {
        var validRiskLevels = new[] { "None", "Low", "Medium", "High", "Critical" };
        return validRiskLevels.Contains(riskLevel, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}