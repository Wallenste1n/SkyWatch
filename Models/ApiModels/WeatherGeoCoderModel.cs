namespace SkyWatch.Models.ApiModels;

//Geo Coder API handler
public class WeatherGeoCoderModel
{
    //name of the city
    public string name { get; set; }
    
    //latitude of the city location
    public double lat { get; set; }
    
    //longitude of the city location
    public double lon { get; set; }
    
    //country of the city
    public string country { get; set; }
    
    //state of the city (if available)
    public string state { get; set; }
}