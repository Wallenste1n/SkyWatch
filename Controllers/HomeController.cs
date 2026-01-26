using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SkyWatch.Models;

namespace SkyWatch.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    //Getting name of the city and post View with the model in async
    //Doing all of it with using WeatherService
    [HttpPost]
    public async Task<IActionResult> Index(string cityName)
    {
        var model = await _weatherService.GetWeatherAsync(cityName ?? "London");
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}