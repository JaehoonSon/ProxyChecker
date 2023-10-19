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

        return type;
    }

    public static string getPath()
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        string path = Directory.GetCurrentDirectory() + "/proxies/proxyToCheck.txt";

        Console.WriteLine($"\nIs the path below a location to your proxy? y/n \n{path}");
        string input = Console.ReadLine();

        if (input == "y")
        {
            return path;
        }
        else if (File.Exists(input))
        {
            return input;
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
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("\nChoose number of threads to run (recommended 200-300): ");
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
        Console.ForegroundColor = ConsoleColor.Red;
        if (msg != null)
        {
            Console.WriteLine(msg);
        }
        Console.WriteLine("Enter any key to exit");
        Console.ReadKey(true);
        Environment.Exit(0);
    }
}

