using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using FraudDetection.Infrastructure.Persistence;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Common;

namespace FraudDetection.API.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptorsToRemove = services
                .Where(d => d.ServiceType.Namespace != null &&
                           (d.ServiceType.Namespace.Contains("EntityFrameworkCore") ||
                            d.ServiceType == typeof(FraudDetectionDbContext) ||
                            d.ServiceType == typeof(IApplicationDbContext)))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            services.AddScoped<IDomainEventDispatcher, TestDomainEventDispatcher>();

            services.AddDbContext<FraudDetectionDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<FraudDetectionDbContext>());
        });
    }
}

public class TestDomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}