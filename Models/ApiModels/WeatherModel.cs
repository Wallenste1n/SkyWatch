using Newtonsoft.Json;

namespace SkyWatch.Models.ApiModels;

public class WeatherModel
{
    //Name of the city
    public string name { get; set; }
    
    //Object of Main class (to get access to "main" directory of API)
    public Main main { get; set; }
    
    //Object of Wind class (to get access to "wind" directory of API)
    public Wind wind { get; set; }
    
    //API for some reason has object in it
    //So in this case it's going to be in the list in the very weird state
    //(should be implemented differently later)
    public List<JsonArrayAttribute> weather { get; set; }
    
    //date text for forecast
    public string dt_txt { get; set; }
}

public class Main 
{
    //Current temperature
    public double temp { get; set; }
    
    //Current temperature feels like
    public double feels_like { get; set; }
    
    //Minimal temperature
    public double temp_min { get; set; }
    
    //Maximum temperature
    public double temp_max { get; set; }
    
    //Current humidity
    public int humidity { get; set; }
}

public class Wind
{
    //speed of the wind
    public double speed { get; set; }
    
    //Wind direction (in degrees)
    public int deg { get; set; }
}

public class Weather
{
    public int id { get; set; }
    
    //State of the weather
    public string main { get; set; }
    
    //Weather description
    public string description { get; set; }
    
    //Mb this code will be used later
    public string icon { get; set; }
}