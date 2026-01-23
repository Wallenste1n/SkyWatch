using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SkyWatch.Models;

namespace SkyWatch.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        //creating a weatherController object to deal with it functions
        WeatherController weatherController = new WeatherController();
        
        //gives view a data from API
        return View(weatherController.GetWeatherData());
    }

    public IActionResult Privacy()
    {
        return View();
        
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}