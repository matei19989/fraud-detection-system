using System.Text.Json;
using System.Text.Json.Serialization;
using FraudDetection.Application.DTOs.RuleConditions;

namespace FraudDetection.Application.Services.RuleEngine;

public class RuleConditionConverter : JsonConverter<RuleCondition>
{
    public override RuleCondition Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        // Read the JSON into a JsonDocument to inspect the Type property
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;

        if (!root.TryGetProperty("Type", out var typeProperty) &&
            !root.TryGetProperty("type", out typeProperty))
        {
            throw new JsonException("RuleCondition must have a 'Type' property");
        }

        var type = typeProperty.GetString();

        // Deserialize based on the type
        var json = root.GetRawText();
        return type?.ToLower() switch
        {
            "amountthreshold" => JsonSerializer.Deserialize<AmountThresholdCondition>(json, options)!,
            "velocity" => JsonSerializer.Deserialize<VelocityCondition>(json, options)!,
            "locationanomaly" => JsonSerializer.Deserialize<LocationAnomalyCondition>(json, options)!,
            "newaccount" => JsonSerializer.Deserialize<NewAccountCondition>(json, options)!,
            "unusualmerchant" => JsonSerializer.Deserialize<UnusualMerchantCondition>(json, options)!,
            "timeofday" => JsonSerializer.Deserialize<TimeOfDayCondition>(json, options)!,
            "amountdeviation" => JsonSerializer.Deserialize<AmountDeviationCondition>(json, options)!,
            "composite" => JsonSerializer.Deserialize<CompositeCondition>(json, options)!,
            _ => throw new JsonException($"Unknown RuleCondition type: {type}")
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        RuleCondition value,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}