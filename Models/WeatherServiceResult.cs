namespace SkyWatch.Models;

//Model mostly to connect WeatherModel and error type handling
public class WeatherServiceResult
{
    public WeatherModel? Weather { get; set; }
    public WeatherErrorType ErrorType { get; set; } = WeatherErrorType.None;
}