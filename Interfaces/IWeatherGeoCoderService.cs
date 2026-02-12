using SkyWatch.Models;

namespace SkyWatch.Interfaces;

public interface IWeatherGeoCoderService
{
    Task<WeatherServiceResult> GetCityCoordinatesAsync(string cityName);
    Task<List<CitySearchResult>> SearchCitiesAsync(string query);
}