namespace SkyWatch.Helpers;

public class WeatherIconHelper
{
    public static string Icon(string iconCode)
    {
        return $"/icons/weather/{iconCode}.png";
    }
}