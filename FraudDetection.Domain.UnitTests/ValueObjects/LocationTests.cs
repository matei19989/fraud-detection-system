using FluentAssertions;
using FraudDetection.Domain.ValueObjects;
using Xunit;

namespace FraudDetection.Domain.UnitTests.ValueObjects;

public class LocationTests
{
    [Fact]
    public void Constructor_WithValidCoordinates_ShouldCreateLocation()
    {
        var location = new Location(40.7128, -74.0060, "USA", "New York", "192.168.1.1");

        location.Latitude.Should().Be(40.7128);
        location.Longitude.Should().Be(-74.0060);
        location.Country.Should().Be("USA");
        location.City.Should().Be("New York");
        location.IpAddress.Should().Be("192.168.1.1");
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    [InlineData(-100)]
    [InlineData(150)]
    public void Constructor_WithInvalidLatitude_ShouldThrowArgumentException(double latitude)
    {
        Action act = () => new Location(latitude, 0);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Latitude must be between -90 and 90*");
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    [InlineData(-200)]
    [InlineData(200)]
    public void Constructor_WithInvalidLongitude_ShouldThrowArgumentException(double longitude)
    {
        Action act = () => new Location(0, longitude);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Longitude must be between -180 and 180*");
    }

    [Theory]
    [InlineData(-90, -180)]
    [InlineData(90, 180)]
    [InlineData(0, 0)]
    public void Constructor_WithBoundaryCoordinates_ShouldCreateLocation(double latitude, double longitude)
    {
        var location = new Location(latitude, longitude);

        location.Latitude.Should().Be(latitude);
        location.Longitude.Should().Be(longitude);
    }

    [Fact]
    public void DistanceInKilometersTo_BetweenSameLocation_ShouldReturnZero()
    {
        var location1 = new Location(40.7128, -74.0060);
        var location2 = new Location(40.7128, -74.0060);

        var distance = location1.DistanceInKilometersTo(location2);

        distance.Should().BeApproximately(0, 0.01);
    }

    [Fact]
    public void DistanceInKilometersTo_BetweenNewYorkAndLondon_ShouldReturnCorrectDistance()
    {
        var newYork = new Location(40.7128, -74.0060);
        var london = new Location(51.5074, -0.1278);

        var distance = newYork.DistanceInKilometersTo(london);

        distance.Should().BeInRange(5500, 5600);
    }

    [Fact]
    public void ToString_WithAllFields_ShouldReturnFormattedString()
    {
        var location = new Location(40.7128, -74.0060, "USA", "New York");

        var result = location.ToString();

        result.Should().Contain("New York");
        result.Should().Contain("USA");
        result.Should().Contain("40.7128");
        result.Should().Contain("-74.0060");
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var location1 = new Location(40.7128, -74.0060, "USA", "New York", "192.168.1.1");
        var location2 = new Location(40.7128, -74.0060, "USA", "New York", "192.168.1.1");

        location1.Should().Be(location2);
        (location1 == location2).Should().BeTrue();
    }
}