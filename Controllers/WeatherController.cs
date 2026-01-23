using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SkyWatch.Models;

namespace SkyWatch.Controllers;

public class WeatherController : Controller
{
    [HttpGet]
    public WeatherModel GetWeatherData()
    {
        //builder for access to API key (for security)
        var builder = WebApplication.CreateBuilder();
       
        //Enters data in url address to get it from API
        string city = "London";
        string? weatherApi = builder.Configuration["Key"];
        string units = "metric";
        string lang = "en";
        string requestedUri = $"https://api.openweathermap.org/data/2.5/weather?" +
                              $"q={city}" +
                              $"&appid={weatherApi}" +
                              $"&units={units}" +
                              $"&lang={lang}";
        
        //Gets API and parse it into string
        HttpClient httpClient = new HttpClient();
        HttpResponseMessage httpResponse = httpClient.GetAsync(requestedUri).Result;
        string response = httpResponse.Content.ReadAsStringAsync().Result;
        
        //Convert it into workable object, and sets info from API to variables from model class
        WeatherModel? weatherModel = JsonConvert.DeserializeObject<WeatherModel>(response);
        return weatherModel;
    }
}