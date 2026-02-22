using SkyWatch.Models.ApiModels;

namespace SkyWatch.Models.ServiceResultsModels;

//Model mostly to connect WeatherModel and error type handling
public class WeatherServiceResult
{
    public WeatherModel? CurrentWeather { get; set; }
    public List<WeatherGeoCoderModel>? GeoCoder { get; set; }
    public WeatherHourlyForecastModel? HourlyForecast { get; set; } 
    public WeatherDailyForecastModel? DailyForecast { get; set; }
    public WeatherErrorType ErrorType { get; set; } = WeatherErrorType.None;
}