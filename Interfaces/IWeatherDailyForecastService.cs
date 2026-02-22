using SkyWatch.Models.ServiceResultsModels;

namespace SkyWatch.Interfaces;

public interface IWeatherDailyForecastService
{
    Task<WeatherServiceResult> GetDailyForecastAsync(double lat, double lon, string units, string lang);
}