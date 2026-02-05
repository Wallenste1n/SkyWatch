using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SkyWatch.Models;
using SkyWatch.Services;

namespace SkyWatch.Controllers;

public class HomeController : Controller
{
    //for getting WeatherService functions
    private readonly WeatherService _weatherService;

    public HomeController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        var model = new WeatherViewModel();

        //Filling information with cookie from user
        model.cityName = Request.Cookies["weather_city"];
        model.units = Request.Cookies["weather_units"] ?? "metric";
        model.lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        //if we get cookie of city name, then we get info for model and return it
        if (!string.IsNullOrEmpty(model.cityName))
        {
            model.Weather = await _weatherService.GetWeatherAsync(
                model.cityName,
                model.units,
                model.lang);  
        }
        
        return View(model);
    }

    //Getting name of the city and post View with the model in async
    //Doing all of it with using WeatherService
    [HttpPost]
    public async Task<IActionResult> Index(string cityName)
    {
        //gives model info about current language in 2 letter format "en", "ru" etc.
        viewModel.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        
        //Gives info for class from API service
        viewModel.Weather = await _weatherService.GetWeatherAsync(
            viewModel.cityName,
            viewModel.units,
            viewModel.lang
            );

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