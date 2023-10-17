using System;
namespace ProxyChecker;

public class ConsoleMessage
{
    public static string proxyOptions()
    {
        string type = String.Empty;
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("1 - HTTP/s");
        Console.WriteLine("2 - socks4");
        Console.WriteLine("3 - socks5");

        var hasInput = false;

        while (!hasInput)
        {
            type = Console.ReadLine();
            switch (type)
            {
                case "1":
                    type = "http://";
                    hasInput = true;
                    break;
                case "2":
                    type = "socks4://";
                    hasInput = true;
                    break;
                case "3":
                    type = "socks5://";
                    hasInput = true;
                    break;
                default:
                    Console.WriteLine("Invalid Input");
                    break;
            }
        }
        Console.ForegroundColor = ConsoleColor.Gray;

        return type;
    }

    public static string getPath()
    {
        string path = Directory.GetCurrentDirectory() + "/proxies/proxyToCheck.txt";

        Console.WriteLine($"Is the path below a location to your proxy? y/n \n{path}");
        bool isCorrect = Console.ReadLine() == "y" ? true : false;

        if (isCorrect)
        {
            return path;
        }
        else
        {
            Console.WriteLine("paste in your file location");
            while (true)
            {
                path = Console.ReadLine();
                if (File.Exists(path))
                {
                    return path;
                }
                else
                {
                    Console.WriteLine("File does not exist");
                }
            
            }
        }
    }

    public static int returnThreadNumber()
    {
        Console.WriteLine("Choose number of threads to run (recommended 20-50): ");
        while (true)
        {
            try
            {
                int bots = Convert.ToInt32(Console.ReadLine());
                return bots;
            }
            catch
            {
                Console.WriteLine("Invalid. Please type a number");
            }
        }
    }

    public static void Exit(string? msg = null)
    {
        if (msg != null)
        {
            Console.WriteLine(msg);
        }
        Console.WriteLine("Enter any key to exit");
        Console.ReadKey(true);
        Environment.Exit(0);
    }
}

