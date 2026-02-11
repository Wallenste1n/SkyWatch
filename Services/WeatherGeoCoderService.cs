using System.Net;
using Newtonsoft.Json;
using SkyWatch.Interfaces;
using SkyWatch.Models;

namespace SkyWatch.Services;

public class WeatherGeoCoderService : IWeatherGeoCoderService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public WeatherGeoCoderService (IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }
    
    //To get coordinates lat and lon by city name
    public async Task<WeatherServiceResult> GetCityCoordinatesAsync(string cityName)
    {
        //Bunch of checks for errors
        if (string.IsNullOrWhiteSpace(cityName))
            return new WeatherServiceResult { ErrorType = WeatherErrorType.CityEmpty };
        
        string? apiKey = _config["Key"];

        if (string.IsNullOrWhiteSpace(apiKey))
            return new WeatherServiceResult { ErrorType = WeatherErrorType.InvalidApiKey };
        
        string url = $"https://api.openweathermap.org/geo/1.0/direct?" +
                     $"q={Uri.EscapeDataString(cityName)}&limit=5&appid={apiKey}";

        HttpResponseMessage? response = null;

        for (int attempt = 1; attempt <= 2; attempt++)
        {
            try
            {
                response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                    break;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new WeatherServiceResult { ErrorType = WeatherErrorType.CityNotFound };
                    case HttpStatusCode.Unauthorized:
                        return new WeatherServiceResult { ErrorType = WeatherErrorType.InvalidApiKey };
                    case HttpStatusCode.TooManyRequests:
                        return new WeatherServiceResult { ErrorType = WeatherErrorType.TooManyRequests };
                }
            }
            catch (HttpRequestException)
            {
                if (attempt == 2)
                {
                    return new WeatherServiceResult { ErrorType = WeatherErrorType.ApiUnavailable };
                }
            }
            catch (Exception)
            {
                return new WeatherServiceResult { ErrorType = WeatherErrorType.Unknown };
            }

            await Task.Delay(300);
        }

        if (response == null || !response.IsSuccessStatusCode)
            return new WeatherServiceResult { ErrorType = WeatherErrorType.ApiUnavailable };
        
        string json = await response.Content.ReadAsStringAsync();
        var geoCoderResult = JsonConvert.DeserializeObject<List<WeatherGeoCoderModel>>(json);

        if (geoCoderResult == null || geoCoderResult.Count == 0)
            return new WeatherServiceResult { ErrorType = WeatherErrorType.CityNotFound };
        
        return new WeatherServiceResult { GeoCoder = geoCoderResult, ErrorType = WeatherErrorType.None};
    }

    //to dynamically search cities in the list by the letter user types
    public async Task<List<CitySearchResult>> SearchCitiesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return new List<CitySearchResult>();
        
        string? apiKey = _config["Key"];

        if (string.IsNullOrWhiteSpace(apiKey))
            return new List<CitySearchResult>();
        string url = $"https://api.openweathermap.org/geo/1.0/direct?" +
                     $"q={Uri.EscapeDataString(query)}&limit=5&appid={apiKey}";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<CitySearchResult>();

            var json = await response.Content.ReadAsStringAsync();
            var apiResult = JsonConvert.DeserializeObject<List<WeatherGeoCoderModel>>(json);

            if (apiResult == null || apiResult.Count == 0)
                return new List<CitySearchResult>();

            return apiResult?.Select(x => new CitySearchResult
            {
                Name = x.name,
                Country = x.country,
                State = x.state,
                Lat = x.lat,
                Lon = x.lon
            }).ToList() ?? [];
        }
        catch (HttpRequestException)
        {
            return new List<CitySearchResult>();
        }
        catch (JsonException)
        {
            return new List<CitySearchResult>();
        }
        catch
        {
            return new List<CitySearchResult>();
        }
    }
}