using SkyWatch.Models;

namespace SkyWatch.Interfaces;

public interface IWeatherHourlyForecastService
{
    Task<WeatherServiceResult> GetHourlyForecast(double lat, double lon, string units, string lang);
}