namespace SkyWatch.Models;

//Enum of error types to handle
public enum WeatherErrorType
{
    None,
    CityEmpty,
    CityNotFound,
    ApiUnavailable,
    InvalidApiKey,
    TooManyRequests,
    Unknown
}