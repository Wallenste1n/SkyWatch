using SkyWatch.Models;
using SkyWatch.Models.ServiceResultsModels;

namespace SkyWatch.Interfaces;

public interface IWeatherHourlyForecastService
{
    Task<WeatherServiceResult> GetHourlyForecast(double lat, double lon, string units, string lang);
}