namespace SkyWatch.Models.ApiModels;

//For daily forecast Api requests
public class WeatherDailyForecastModel
{
    public List<ListOfData>? list { get; set; }
}

public class ListOfData
{
    public long dt { get; set; }
    public Temperature temp { get; set; }
    public List<Weather>? weather { get; set; }
}

public class Temperature
{
    public double day { get; set; }
    public double min { get; set; }
    public double max { get; set; }
}