using SkyWatch.Models;

namespace SkyWatch.Interfaces;

//interface for implementing functions in WeatherService
public interface IWeatherService
{
    //for getting api and it's data in async
    Task<WeatherModel> GetWeatherAsync(string city);
}