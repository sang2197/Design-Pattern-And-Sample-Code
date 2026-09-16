using Exercises.Observer.Example3;
using Xunit;

namespace Observer.Tests;

public class Example3Tests
{
    [Fact]
    public void MultipleDisplays_ReactDifferentlyToSameMeasurement()
    {
        var station = new WeatherStation();
        var current = new CurrentConditionsDisplay();
        var stats = new StatisticsDisplay();

        station.Subscribe(current);
        station.Subscribe(stats);

        station.SetMeasurements(25, 65);
        station.SetMeasurements(27, 70);

        Assert.Equal(27, current.LastTemperature);
        Assert.Equal(70, current.LastHumidity);
        Assert.Equal(26, stats.AverageTemperature);
    }
}
