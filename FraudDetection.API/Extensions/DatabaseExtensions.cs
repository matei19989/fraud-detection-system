using FraudDetection.Infrastructure.Data;
using FraudDetection.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Extensions;

public static class DatabaseExtensions
{
    public static async Task<IApplicationBuilder> InitializeDatabaseAsync(
        this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<FraudDetectionDbContext>>();

        try
        {
            var context = services.GetRequiredService<FraudDetectionDbContext>();

            // Apply pending migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");

            // Seed fraud rules
            var seeder = new FraudRuleSeeder(
                context,
                services.GetRequiredService<ILogger<FraudRuleSeeder>>());

            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }

        return app;
    }
}