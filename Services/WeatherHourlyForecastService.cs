using Newtonsoft.Json;
using SkyWatch.Interfaces;
using SkyWatch.Models;
using SkyWatch.Models.ApiModels;
using SkyWatch.Models.ServiceResultsModels;

namespace SkyWatch.Services;

public class WeatherHourlyForecastService : IWeatherHourlyForecastService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public WeatherHourlyForecastService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }
    
    public async Task<WeatherServiceResult> GetHourlyForecast(double lat, double lon, string units, string lang)
    {
        string? apiKey = _config["Key"];
        
        if (string.IsNullOrWhiteSpace(apiKey))
            return new WeatherServiceResult { ErrorType = WeatherErrorType.InvalidApiKey };
        
        string cnt = "24";
        string url = $"https://pro.openweathermap.org/data/2.5/forecast/hourly?" +
                     $"lat={lat}&lon={lon}&appid={apiKey}&cnt={cnt}&units={units}&lang={lang}";

        var response = await _httpClient.GetAsync(url);
        string json = await response.Content.ReadAsStringAsync();
        var forecast = JsonConvert.DeserializeObject<WeatherHourlyForecastModel>(json);

        return new WeatherServiceResult { HourlyForecast = forecast, ErrorType = WeatherErrorType.None };
    }
}