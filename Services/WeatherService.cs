using System.Net;
using Newtonsoft.Json;
using SkyWatch.Interfaces;
using SkyWatch.Models;

namespace SkyWatch.Services;

//Implements IWeatherService
public class WeatherService : IWeatherService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    private readonly IHttpContextAccessor _httpContext;

    //For getting config and httpClient classes works
    //+ to lower dependencies in Controllers
    public WeatherService(IConfiguration config, 
        HttpClient httpClient, 
        ILogger<WeatherService> logger, 
        IHttpContextAccessor httpContext)
    {
        _config = config;
        _httpClient = httpClient;
        _logger = logger;
        _httpContext = httpContext;
    }

    //Getting information from API response and send its info to WeatherModel in async
    //doing attempts to reconnect if network gone, if yes, then trying to fallback
    //Handles possible exceptions
    public async Task<WeatherServiceResult> GetWeatherAsync(double lat, double lon, string units, string lang)
    {
        //apiKey is getting Key for WeatherAPI from the JSON file
        string? apiKey = _config["Key"];
        string url = $"https://api.openweathermap.org/data/2.5/weather" +
                     $"?lat={lat}&lon={lon}&appid={apiKey}&units={units}&lang={lang}";

        HttpResponseMessage? response = null;
        
        //Loop for attempt to get data from API (cum, thanks galactica)
        //And handles some possible exceptions
        for (int attempt = 1; attempt < 3; attempt++)
        {
            try
            {
                //log info about attempt
                _logger.LogInformation("Weather API attempt {Attempt}", attempt);
                response = await _httpClient.GetAsync(url);
                
                //if response successful, then (cum, thanks galactica) loop breaks
                if(response.IsSuccessStatusCode) 
                    break;
                
                //if not, gives some error
                switch (response.StatusCode)
                {
                    //if city is not found gives CityNotFound error
                    case HttpStatusCode.NotFound:
                        return new WeatherServiceResult { ErrorType = WeatherErrorType.CityNotFound };
                    //if API in not valid then gives 
                    case HttpStatusCode.Unauthorized:
                        return new WeatherServiceResult { ErrorType = WeatherErrorType.InvalidApiKey };
                }

                if ((int)response.StatusCode == 429)
                    return new WeatherServiceResult { ErrorType = WeatherErrorType.TooManyRequests };
            }
            catch (HttpRequestException exception)
            {
                //if some attempts are failed gives log message (the fuck, thanks galactika)
                _logger.LogWarning(exception, "API attempt {Attempt} failed",attempt);
            }
            catch (Exception exception)
            {
                //log unexpected exceptions
                _logger.LogCritical(exception, "Unexpected error");
                return new WeatherServiceResult { ErrorType = WeatherErrorType.Unknown };
            }

            //Waits 300ms before next attempt
            await Task.Delay(300);
        }
        
        //if response from API is not success, then trying to fallback
        if (response == null || !response.IsSuccessStatusCode)
        {
            _logger.LogError("All API attempts failed. Using Fallback");
            return TryFallback(lat, lon, units);
        }
        
        var json = await response.Content.ReadAsStringAsync();
        var weatherResult = JsonConvert.DeserializeObject<WeatherModel>(json);

        //Saves Fallback states 
        SaveFallback(lat, lon, units, weatherResult);
        
        return new WeatherServiceResult { Weather = weatherResult };
    }

    //Trying to fallback code
    //That means if something goes wrong, program trying to set it's state to the previous one (that works)
    private WeatherServiceResult TryFallback(double lat, double lon, string units)
    {
        //Looking for weather_fallback cookie
        var context = _httpContext.HttpContext;
        var cookie = context?.Request.Cookies["weather_fallback"];

        //if weather_fallback cookie doesn't exist then gives error 
        if (cookie == null)
            return new WeatherServiceResult { ErrorType = WeatherErrorType.ApiUnavailable };

        //if fallback is not equal to entered city and units, then gives error
        var fallback = JsonConvert.DeserializeObject<FallbackWeather>(cookie);
        const double epsilon = 0.0001;
        
        if (Math.Abs(fallback.Lat - lat) > epsilon || 
            Math.Abs(fallback.Lon - lon) > epsilon || 
            fallback.Units != units)
            return new WeatherServiceResult { ErrorType = WeatherErrorType.ApiUnavailable };
                
        _logger.LogInformation("Fallback weather used");

        return new WeatherServiceResult
        {
            Weather = fallback.Weather
        };
    }
    
    //Saves state of the data for possible fallback
    private void SaveFallback(double lat, double lon, string units, WeatherModel weather)
    {
        //if there is no context, then function will not save anything
        var context = _httpContext.HttpContext;
        if(context == null) return;

        var fallback = new FallbackWeather
        {
            Lat = lat,
            Lon = lon,
            Units = units,
            Weather = weather
        };
        
        //Sends data to cookie for using in possible fallback 
        context.Response.Cookies.Append("weather_fallback", JsonConvert.SerializeObject(fallback),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                IsEssential = true
            });
    }
}