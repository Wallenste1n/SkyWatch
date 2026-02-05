namespace SkyWatch.Models;

public class WeatherViewModel
{
    //UI state
    public string cityName { get; set; } = "London";
    public string units { get; set; } = "metric";
    public string lang { get; set; } = "en";
    
    public WeatherModel? Weather { get; set; }

    //switches API metrics to it units
    public string TemperatureUnit =>
        units switch
        {
            "metric" => "°C",
            "imperial" => "°F",
            "standard" => "K",
            _ => ""
        };
}