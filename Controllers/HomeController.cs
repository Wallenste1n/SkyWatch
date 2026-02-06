using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using SkyWatch.Helper;
using SkyWatch.Interfaces;
using SkyWatch.Models;

namespace SkyWatch.Controllers;

public class HomeController : Controller
{
    //for getting WeatherService functions
    private readonly IWeatherService _weatherService;

    public HomeController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }
    
    //Gets info of cookies and return weather data
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new WeatherViewModel
        {
            //Filling information with cookie from user
            cityName = Request.Cookies["weather_city"],
            units = Request.Cookies["weather_units"] ?? "metric",
            lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName
        };

        //if we get cookie of city name, then we get info for model and return it
        if (!string.IsNullOrWhiteSpace(model.cityName))
        {
            var result = await _weatherService.GetWeatherAsync(
                model.cityName,
                model.units,
                model.lang);

            //Sets info to Weather class and type for Error to handle it
            model.Weather = result.Weather;
            model.ErrorType = result.ErrorType;
            
            //Gets geographical direction (East, West etc.) as a key for localization
            model.WindDirectionKey = WindDirectionHelper.GetDirection(model.Weather.wind.deg);
        }
        
        return View(model);
    }
    
    //Filling model class with language info
    //Creates cookie for keeping chosen city and units in browser memory
    //Doing all of it with using WeatherService
    [HttpPost]
    public async Task<IActionResult> Index(WeatherViewModel viewModel)
    {
        //gives model info about current language in 2 letter format "en", "ru" etc.
        viewModel.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        
        //Gives info for class from API service
        var result = await _weatherService.GetWeatherAsync(
            viewModel.cityName,
            viewModel.units,
            viewModel.lang
            );

        //Sets info to Weather class and type for Error to handle it
        viewModel.Weather = result.Weather;
        viewModel.ErrorType = result.ErrorType;
        
        //if user didn't leave input to be blank returns View without API info
        if (viewModel.ErrorType == WeatherErrorType.CityEmpty)
            return View(viewModel);

        //Gets geo direction (East, West etc.) as a key for localization
        viewModel.WindDirectionKey = WindDirectionHelper.GetDirection(viewModel.Weather.wind.deg);
        
        //Adds weather_city cookie to the client for storing chosen city name 
        Response.Cookies.Append("weather_city", viewModel.cityName, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
        
        //Adds weather_units cookie to the client for chosen storing units 
        Response.Cookies.Append("weather_units", viewModel.units, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
        
        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}