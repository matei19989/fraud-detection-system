namespace FraudDetection.Domain.ValueObjects;

public sealed record Location
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? IpAddress { get; init; }

    public Location(double latitude, double longitude, string? country = null, string? city = null, string? ipAddress = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
        Country = country;
        City = city;
        IpAddress = ipAddress;
    }

    public double DistanceInKilometersTo(Location other)
    {
        // Haversine formula to calculate distance between two points on Earth
        const double earthRadiusKm = 6371;

        var lat1Rad = Latitude * Math.PI / 180;
        var lat2Rad = other.Latitude * Math.PI / 180;
        var deltaLat = (other.Latitude - Latitude) * Math.PI / 180;
        var deltaLon = (other.Longitude - Longitude) * Math.PI / 180;

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    public override string ToString()
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(City)) parts.Add(City);
        if (!string.IsNullOrEmpty(Country)) parts.Add(Country);
        parts.Add($"({Latitude:F4}, {Longitude:F4})");

        return string.Join(", ", parts);
    }
}