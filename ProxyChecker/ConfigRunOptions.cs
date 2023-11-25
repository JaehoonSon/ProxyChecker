using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChecker;

internal class ConfigRunOptions
{
    public static string _sourceToRead;
    public static bool _isFile;
    public static string _pathToWrite = Path.Combine(
        Directory.GetCurrentDirectory(),
        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
        );
    public static string? _proxyType = "http://";
    public static int _threads = 20;

    public static async Task CommandLineOptions(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--help" or "-h":
                    PrintUsage();
                    Exit();
                    break;
                case "--print":
                    i++;
                    if (!(i < args.Length))
                        Exit("Print file not found");
                    await PrintFile(args[i]);
                    Exit();
                    return;
                case "--read" or "-r":
                    i++;

                    if (i < args.Length &&
                        CheckValidUrl(args[i]))
                    {
                        _sourceToRead = args[i];
                        _isFile = false;
                        break;
                    }

                    if (i < args.Length &&
                        !File.Exists(args[i])
                        )
                        Exit("Read file not found");
                    _sourceToRead = args[i];
                    _isFile = true;
                    break;
                case "--write" or "-w":
                    i++;
                    if (!(i < args.Length) ||
                        File.Exists(Path.Combine(
                            Directory.GetCurrentDirectory(),
                            args[i])
                        ))
                        Exit("Invalid writing destination");

                    _pathToWrite = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        args[i]);
                    break;
                case "--proxy-type" or "-p":
                    i++;
                    if (!(i < args.Length))
                        Exit("proxy-type not found");

                    _proxyType = args[i] switch
                    {
                        "http" or "https" => "https://",
                        "socks4" => "socks4://",
                        "socks5" => "socks5://",
                        _ => null
                    };
                    break;
                case "--threads" or "-t":
                    i++;
                    if (i < args.Length && int.TryParse(args[i], out var res))
                    {
                        _threads = res;
                        break;
                    }
                    Exit("Enter a valid number for threads");
                    break;
            }
        }
        if (string.IsNullOrEmpty(_sourceToRead) ||
            string.IsNullOrEmpty(_pathToWrite) ||
            string.IsNullOrEmpty(_proxyType)
            )
        {
            Exit("Please specify read and write destination or invalid proxy type");
        }
    }

    public static void PrintUsage()
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("ProxyChecker by Knuceles\n");
        Console.WriteLine("Proxychecker [OPTIONS] value");
        Console.WriteLine("Options:");
        Console.WriteLine("--read | -r".PadRight(20) + ": Species file source or url");
        Console.WriteLine("--write | -w".PadRight(20) +  ": Species output file destination (default file would be {unix}.txt)");
        Console.WriteLine("--threads | -t".PadRight(20) +  ": Number of threads (5 by default)");
        Console.WriteLine("--proxy-type | -p".PadRight(20) +  ": socks4, socks5, http/s (http by default)");
        Console.WriteLine("--print".PadRight(20) + ": Prints a file content");
        Console.WriteLine("--help | -h".PadRight(20) +  ": This output");
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static async Task PrintFile(string path)
    {
        if (File.Exists(path))
            Exit(await File.ReadAllTextAsync(path));

        var anotherPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            path);

        if (File.Exists(anotherPath))
            Exit(await File.ReadAllTextAsync(anotherPath));
        Exit("Cannot print file content");
    }

    public static bool CheckValidUrl(string url)
        => Uri.IsWellFormedUriString(url, UriKind.Absolute);

    public static void Exit(string? msg = null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        if (msg != null)
        {
            Console.WriteLine(msg);
        }

        Console.ForegroundColor = ConsoleColor.Gray;
        Environment.Exit(0);
    }
}
