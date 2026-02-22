using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using SkyWatch.Helpers;
using SkyWatch.Interfaces;
using SkyWatch.Models;

namespace SkyWatch.Controllers;

public class HomeController : Controller
{
    //for getting WeatherService functions
    private readonly IWeatherService _weatherService;
    private readonly IWeatherGeoCoderService _geoCoderService;
    private readonly IWeatherHourlyForecastService _hourlyForecastService;
    private readonly IWeatherDailyForecastService _dailyForecastService;

    public HomeController
        (
            IWeatherService weatherService, 
            IWeatherGeoCoderService geoCoderService, 
            IWeatherHourlyForecastService hourlyForecastService, 
            IWeatherDailyForecastService dailyForecastService)
    {
        _weatherService = weatherService;
        _geoCoderService = geoCoderService;
        _hourlyForecastService = hourlyForecastService;
        _dailyForecastService = dailyForecastService;
    }
    
    //Gets info of cookies and return weather data
    [HttpGet] 
    public async Task<IActionResult> Index()
    {
        double.TryParse(Request.Cookies["city_lat"],
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var lat);
        
        double.TryParse(Request.Cookies["city_lon"],
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var lon);
        
        var model = new WeatherViewModel
        {
            //Filling information with cookie from user
            cityName = Request.Cookies["weather_city"] ?? "",
            Lat = lat,
            Lon = lon,
            units = Request.Cookies["weather_units"] ?? "metric",
            lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
            CityDisplayName = Request.Cookies["weather_city"]
        };
        
        //if we get cookie of city name, then we get info for model and return it
        if (Math.Abs(lat) > 0.001 && Math.Abs(lon) > 0.001)
        {
            var resultCurrentWeather = await _weatherService.GetWeatherAsync(
                lat,
                lon,
                model.units,
                model.lang
            );

            var resultHourlyForecast = await _hourlyForecastService.GetHourlyForecast(
                lat,
                lon,
                model.units,
                model.lang
            );

            var resultDailyForecast = await _dailyForecastService.GetDailyForecastAsync(
                lat,
                lon,
                model.units,
                model.lang
            );

            //Sets info to Weather class and type for Error to handle it
            model.CurrentWeather = resultCurrentWeather.CurrentWeather;
            model.HourlyForecastWeather = resultHourlyForecast.HourlyForecast;
            model.DailyForecastWeather = resultDailyForecast.DailyForecast;
            model.ErrorType = resultCurrentWeather.ErrorType;
            
            //Gets geographical direction (East, West etc.) as a key for localization
            if (model.CurrentWeather != null) 
                model.WindDirectionKey = WindDirectionHelper.GetDirection(model.CurrentWeather.wind.deg);
        }
        
        return View(model);
    }
    
    //Filling model class with language info
    //Creates cookie for keeping chosen city and units in browser memory
    //Doing all of it with using WeatherService
    [HttpPost]
    public async Task<IActionResult> Index(WeatherViewModel viewModel)
    {
        viewModel.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        double lat = 0;
        double lon = 0;
        string? cityDisplayName = null;
        string? country = null;
        string? state = null;

        if (!string.IsNullOrWhiteSpace(viewModel.cityName))
        {
            //Checks if Geo Coder responds  
            var geoCoderResult = await _geoCoderService.GetCityCoordinatesAsync(viewModel.cityName);
            if (geoCoderResult == null || geoCoderResult.GeoCoder.Count == 0)
            {
                viewModel.ErrorType = WeatherErrorType.CityNotFound;
                return View(viewModel);
            }
            
            var geo = geoCoderResult.GeoCoder[0];
            
            //gets data from Geo Coder
            lat = geo.lat;
            lon = geo.lon;
            cityDisplayName = geo.name;
            country = geo.country;
            state = geo.state;
        }
        else
        {
            //trying to get info from cookie, if changes units or city
            double.TryParse(Request.Cookies["city_lat"],
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out lat);

            double.TryParse(Request.Cookies["city_lon"],
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out lon);

            cityDisplayName = Request.Cookies["weather_city"];
        }

        if (lat == 0 || lon == 0)
        {
            viewModel.ErrorType = WeatherErrorType.CityEmpty;
            return View(viewModel);
        }

        //Getting info from current Weather API
        var resultCurrentWeather = await _weatherService.GetWeatherAsync(
            lat,
            lon,
            viewModel.units ?? "metric",
            viewModel.lang
        );

        var resultHourlyForecast = await _hourlyForecastService.GetHourlyForecast(
            lat,
            lon,
            viewModel.units ?? "metric",
            viewModel.lang
        );

        var resultDailyForecast = await _dailyForecastService.GetDailyForecastAsync(
            lat,
            lon,
            viewModel.units ?? "metric",
            viewModel.lang
        );

        viewModel.CurrentWeather = resultCurrentWeather.CurrentWeather;
        viewModel.HourlyForecastWeather = resultHourlyForecast.HourlyForecast;
        viewModel.DailyForecastWeather = resultDailyForecast.DailyForecast;
        viewModel.ErrorType = resultCurrentWeather.ErrorType;

        if (viewModel.CurrentWeather == null)
            return View(viewModel);

        //Fills info to view it on the site
        viewModel.WindDirectionKey = WindDirectionHelper.GetDirection(viewModel.CurrentWeather.wind.deg);
        
        viewModel.Lat = lat;
        viewModel.Lon = lon;
        viewModel.CityDisplayName = cityDisplayName;
        viewModel.Country = country;
        viewModel.State = state;
        
        //Timer for cookies
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        };
        
        //Saves cookies 
        Response.Cookies.Append("weather_city", cityDisplayName ?? "", cookieOptions);
        Response.Cookies.Append("city_lat", lat.ToString(CultureInfo.InvariantCulture), cookieOptions);
        Response.Cookies.Append("city_lon", lon.ToString(CultureInfo.InvariantCulture), cookieOptions);
        Response.Cookies.Append("weather_units", viewModel.units ?? "metric", cookieOptions);

        viewModel.cityName = "";
        
        return View(viewModel);
    }

    //Sends info about cities in the list to partial _CitySuggestions
    [HttpGet]
    public async Task<IActionResult> CitySuggestionsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Content(string.Empty);
        
        var cities = await _geoCoderService.SearchCitiesAsync(query);

        if (cities.Count == 0)
            return Content(string.Empty);
        
        return PartialView("_CitySuggestions", cities);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}