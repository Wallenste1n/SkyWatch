using Newtonsoft.Json;

namespace SkyWatch.Models;

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
}

public class Main 
{
    //Current temperature
    public double temp { get; set; }
    
    //Current temperature feels like
    public double feels_like { get; set; }
    
    //Current humidity
    public int humidity { get; set; }
}

public class Wind
{
    //speed of the wind
    public double speed;
}