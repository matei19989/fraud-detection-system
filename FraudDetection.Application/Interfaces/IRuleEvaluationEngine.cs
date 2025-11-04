using FraudDetection.Domain.Entities;

namespace FraudDetection.Application.Interfaces;

public interface IRuleEvaluationEngine
{
    Task<RuleEvaluationResult> EvaluateRuleAsync(
        FraudRule rule,
        Transaction transaction,
        Account account,
        CancellationToken cancellationToken = default);
}