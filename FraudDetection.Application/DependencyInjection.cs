using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using FraudDetection.Application.Behaviors;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Services;
using FraudDetection.Application.Services.RuleEngine;

namespace FraudDetection.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddMemoryCache();

        services.AddScoped<IFraudDetectionService, FraudDetectionService>();
        services.AddScoped<IRuleEvaluationEngine, RuleEvaluationEngine>();

        return services;
    }
}
