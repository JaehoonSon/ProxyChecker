using System;
namespace ProxyChecker;

public class Directories
{
    public static readonly string READING_LOCATION;
    public static readonly string WRITING_LOCATION = WriteLocation();

    private static string WriteLocation()
    {
        var location = Directory.GetCurrentDirectory() + "/WorkingProxies.txt";
        if (!File.Exists(location))
        {
            File.Create(location);
        }
        return location;
    }


}

