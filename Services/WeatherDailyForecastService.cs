using Newtonsoft.Json;
using SkyWatch.Interfaces;
using SkyWatch.Models;
using SkyWatch.Models.ApiModels;
using SkyWatch.Models.ServiceResultsModels;

namespace SkyWatch.Services;

public class WeatherDailyForecastService : IWeatherDailyForecastService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public WeatherDailyForecastService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }
    
    //Gets info from daily forecast API
    public async Task<WeatherServiceResult> GetDailyForecastAsync(double lat, double lon, string units, string lang)
    {
        string? apiKey = _config["Key"];

        if (string.IsNullOrWhiteSpace(apiKey))
            return new WeatherServiceResult { ErrorType = WeatherErrorType.InvalidApiKey };
        
        string cnt = "7";
        string url = $"https://api.openweathermap.org/data/2.5/forecast/daily?" +
                     $"lat={lat}&lon={lon}&cnt={cnt}&appid={apiKey}&lang={lang}&units={units}";

        var response = await _httpClient.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        var dailyForecast = JsonConvert.DeserializeObject<WeatherDailyForecastModel>(json);

        return new WeatherServiceResult { DailyForecast = dailyForecast };
    }
}