namespace SkyWatch.Helpers;

public class WindDirectionHelper
{
    //Gets names of geographical directions by it's degrees
    public static string GetDirection(double degrees)
    {
        degrees = (degrees + 360) % 360;

        switch (degrees)
        {
            case >= 337.5:
            case < 22.5:
                return "Wind_North";
            case >= 22.5 and < 67.5:
                return "Wind_NorthEast";
            case >= 67.5 and < 112.5:
                return "Wind_East";
            case >= 112.5 and < 157.5:
                return "Wind_SouthEast";
            case >= 157.5 and < 202.5:
                return "Wind_South";
            case >= 202.5 and < 247.5:
                return "Wind_SouthWest";
            case >= 247.5 and < 295.5:
                return "Wind_West";
            default:
                return "Wind_NorthWest";
        }
    }
}