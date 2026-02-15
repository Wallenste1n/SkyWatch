using SkyWatch.Models;
using SkyWatch.Models.ServiceResultsModels;

namespace SkyWatch.Interfaces;

//interface for implementing functions in WeatherService
public interface IWeatherService
{
    //for getting api and it's data in async
    Task<WeatherServiceResult> GetWeatherAsync(double lat, double lon, string units, string lang);
}