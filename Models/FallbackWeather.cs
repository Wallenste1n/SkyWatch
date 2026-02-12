namespace SkyWatch.Models;

//Contains info for fallback
public class FallbackWeather
{
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string Units { get; set; }
    public WeatherModel? Weather { get; set; }
}