using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FraudDetection.Infrastructure.Persistence;

public class FraudDetectionDbContextFactory : IDesignTimeDbContextFactory<FraudDetectionDbContext>
{
    public FraudDetectionDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FraudDetectionDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=FraudDetectionDb;Trusted_Connection=true;TrustServerCertificate=true;");

        return new FraudDetectionDbContext(optionsBuilder.Options, null!);
    }
}
