using Newtonsoft.Json;
using SkyWatch.Interfaces;
using SkyWatch.Models;

namespace SkyWatch.Services;

//Implements IWeatherService
public class WeatherService : IWeatherService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    //For getting config and httpClient classes works
    //+ to lower dependencies in Controllers
    public WeatherService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    //Getting information from API response and send its info to WeatherModel in async
    public async Task<WeatherModel> GetWeatherAsync(string city, string units, string lang)
    {
        //apiKey is getting Key for WeatherAPI from the JSON file
        string? apiKey = _config["Key"];
        string units = "metric";
        string lang = "en";
        string url = $"https://api.openweathermap.org/data/2.5/weather" +
                     $"?q={city}&appid={apiKey}&units={units}&lang={lang}";

        var response = await _httpClient.GetStringAsync(url);
        return JsonConvert.DeserializeObject<WeatherModel>(response);
    }
}