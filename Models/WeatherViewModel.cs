using SkyWatch.Models.ApiModels;

namespace SkyWatch.Models;

public class WeatherViewModel
{
    //UI state
    public string cityName { get; set; } = "London";
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string units { get; set; } = "metric";
    public string lang { get; set; } = "en";
    public string? CityDisplayName { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    
    //Keys for localization geographical directions 
    public string? WindDirectionKey { get; set; }
    public WeatherModel? CurrentWeather { get; set; }
    public WeatherHourlyForecastModel? HourlyForecastWeather { get; set; }
    
    public WeatherDailyForecastModel? DailyForecastWeather { get; set; }

    //Contains Error types for handling them
    public WeatherErrorType ErrorType { get; set; } = WeatherErrorType.None;

    //switches API metrics to it units
    public string TemperatureUnit =>
        units switch
        {
            "metric" => "°C",
            "imperial" => "°F",
            "standard" => "K",
            _ => ""
        };
    
    //Converts API metrics into speed units
    public string SpeedWindUnit =>
        units switch
        {
            "metric" => "m/s",
            "imperial" => "m/h",
            "standard" => "m/s",
            _ => ""
        };
}