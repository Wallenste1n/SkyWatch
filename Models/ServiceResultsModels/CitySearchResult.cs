namespace SkyWatch.Models.ServiceResultsModels;

//To contain Result from API
public class CitySearchResult
{
    public string Name { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }

    public string DisplayName =>
        string.IsNullOrEmpty(State)
            ? $"{Name}, {Country}"
            : $"{Name}, {Country}, {State}";
}