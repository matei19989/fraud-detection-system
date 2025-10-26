namespace FraudDetection.Domain.ValueObjects;

public sealed record MerchantInfo
{
    public string MerchantId { get; init; }
    public string MerchantName { get; init; }
    public string Category { get; init; }

    public MerchantInfo(string merchantId, string merchantName, string category)
    {
        if (string.IsNullOrWhiteSpace(merchantId))
            throw new ArgumentException("Merchant ID cannot be null or empty", nameof(merchantId));

        if (string.IsNullOrWhiteSpace(merchantName))
            throw new ArgumentException("Merchant name cannot be null or empty", nameof(merchantName));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be null or empty", nameof(category));

        MerchantId = merchantId;
        MerchantName = merchantName;
        Category = category;
    }

    public override string ToString() => $"{MerchantName} ({Category})";
}