using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FraudDetection.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        // TODO: Add pipeline behaviors (validation, logging, transaction)

        return services;
    }
}