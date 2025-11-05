using System.Net;
using FluentAssertions;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Domain.Entities;
using FraudDetection.Domain.Enums;
using Xunit;

namespace FraudDetection.API.IntegrationTests;

public class FraudDetectionIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public FraudDetectionIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTransaction_WithValidData_ShouldReturn201AndCreateTransaction()
    {
        var command = new CreateTransactionCommand
        {
            AccountId = "ACC123",
            Amount = 100.50m,
            Currency = "USD",
            Type = "Purchase",
            MerchantId = "MERCH001",
            MerchantName = "Test Store",
            MerchantCategory = "Retail",
            Latitude = 40.7128,
            Longitude = -74.0060,
            Country = "US",
            City = "New York",
            IpAddress = "192.168.1.1",
            DeviceId = "DEV123",
            Description = "Test purchase"
        };

        var response = await _client.PostAsJsonAsync("/api/transactions", command);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Error Response: {errorContent}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Error: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<TransactionDto>();
        result.Should().NotBeNull();
        result!.AccountId.Should().Be("ACC123");
        result.Amount.Should().Be(100.50m);
        result.Status.Should().Be("Pending");

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var transaction = await dbContext.Transactions.FindAsync(result.Id);
        transaction.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTransaction_WithInvalidAmount_ShouldReturn400()
    {
        var command = new CreateTransactionCommand
        {
            AccountId = "ACC123",
            Amount = -100m,
            Currency = "USD",
            Type = "Purchase",
            MerchantId = "MERCH001",
            MerchantName = "Test Store",
            MerchantCategory = "Retail",
            Latitude = 40.7128,
            Longitude = -74.0060
        };

        var response = await _client.PostAsJsonAsync("/api/transactions", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTransaction_WithInvalidCurrency_ShouldReturn400()
    {
        var command = new CreateTransactionCommand
        {
            AccountId = "ACC123",
            Amount = 100m,
            Currency = "AB",
            Type = "Purchase",
            MerchantId = "MERCH001",
            MerchantName = "Test Store",
            MerchantCategory = "Retail",
            Latitude = 40.7128,
            Longitude = -74.0060
        };

        var response = await _client.PostAsJsonAsync("/api/transactions", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTransactionById_WithExistingId_ShouldReturn200()
    {
        var createCommand = new CreateTransactionCommand
        {
            AccountId = "ACC456",
            Amount = 250m,
            Currency = "USD",
            Type = "Purchase",
            MerchantId = "MERCH002",
            MerchantName = "Another Store",
            MerchantCategory = "Electronics",
            Latitude = 34.0522,
            Longitude = -118.2437
        };

        var createResponse = await _client.PostAsJsonAsync("/api/transactions", createCommand);
        var created = await createResponse.Content.ReadFromJsonAsync<TransactionDto>();

        var response = await _client.GetAsync($"/api/transactions/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<TransactionDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.AccountId.Should().Be("ACC456");
    }

    [Fact]
    public async Task GetTransactionById_WithNonExistentId_ShouldReturn404()
    {
        var nonExistentId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/transactions/{nonExistentId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task InvestigateAlert_WithValidData_ShouldReturn200AndUpdateStatus()
    {
        Guid alertId;

        // Setup: Create alert in database FIRST
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var transactionId = Guid.NewGuid();
            var alert = new FraudAlert(
                transactionId,
                "Test Rule",
                FraudRiskLevel.High,
                85.0,
                "Suspicious activity detected",
                ruleId: Guid.NewGuid()
            );

            dbContext.FraudAlerts.Add(alert);
            await dbContext.SaveChangesAsync(default);
            alertId = alert.Id;
        }

        // Act: Make HTTP request
        var request = new { InvestigatedBy = "investigator@example.com" };
        var response = await _client.PutAsJsonAsync($"/api/fraudalerts/{alertId}/investigate", request);

        // Assert: Check response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify: Check database was updated
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            var updatedAlert = await dbContext.FraudAlerts.FindAsync(alertId);
            updatedAlert.Should().NotBeNull();
            updatedAlert!.Status.Should().Be(AlertStatus.Investigating);
            updatedAlert.ReviewedBy.Should().Be("investigator@example.com");
        }
    }

    [Fact]
    public async Task CreateFraudRule_WithValidData_ShouldReturn201()
    {
        var command = new CreateFraudRuleCommand
        {
            Name = "High Value Transaction Rule",
            Description = "Flags transactions over $10,000",
            RiskLevel = "High",
            RuleType = "AmountThreshold",
            ConditionsJson = "{\"threshold\": 10000}",
            Priority = 1
        };

        var response = await _client.PostAsJsonAsync("/api/fraudrules", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<FraudRuleDto>();
        result.Should().NotBeNull();
        result!.Name.Should().Be("High Value Transaction Rule");
        result.IsActive.Should().BeTrue();
        result.TimesTriggered.Should().Be(0);
    }

    [Fact]
    public async Task UpdateFraudRule_WithInvalidRiskLevel_ShouldReturn400()
    {
        var createCommand = new CreateFraudRuleCommand
        {
            Name = "Test Rule",
            Description = "Test Description",
            RiskLevel = "Medium",
            RuleType = "VelocityCheck",
            ConditionsJson = "{}",
            Priority = 1
        };

        var createResponse = await _client.PostAsJsonAsync("/api/fraudrules", createCommand);
        var created = await createResponse.Content.ReadFromJsonAsync<FraudRuleDto>();

        var updateRequest = new
        {
            Name = "Updated Rule",
            Description = "Updated Description",
            RiskLevel = "InvalidLevel"
        };

        var response = await _client.PutAsJsonAsync($"/api/fraudrules/{created!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

public record TransactionDto
{
    public Guid Id { get; init; }
    public string AccountId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string MerchantId { get; init; } = string.Empty;
    public string MerchantName { get; init; } = string.Empty;
    public string MerchantCategory { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public double FraudScore { get; init; }
    public DateTime TransactionDate { get; init; }
    public string? DeviceId { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record FraudRuleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public int Priority { get; init; }
    public string RuleType { get; init; } = string.Empty;
    public int TimesTriggered { get; init; }
    public DateTime? LastTriggeredAt { get; init; }
}