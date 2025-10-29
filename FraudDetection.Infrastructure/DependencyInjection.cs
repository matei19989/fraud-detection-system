using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FraudDetection.Application.Interfaces;
using FraudDetection.Infrastructure.Persistence;
using FraudDetection.Infrastructure.Services;

namespace FraudDetection.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        
        services.AddScoped<IRealtimeNotificationService, SignalRNotificationService>();

        // Add DbContext
        services.AddDbContext<FraudDetectionDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(FraudDetectionDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<FraudDetectionDbContext>());

        return services;
    }
}