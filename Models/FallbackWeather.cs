namespace SkyWatch.Models;

//Contains info for fallback
public class FallbackWeather
{
    public string City { get; set; }
    public string Units { get; set; }
    public WeatherModel? Weather { get; set; }
}