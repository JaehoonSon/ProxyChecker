using System.Collections.Concurrent;
using System.Net;

namespace ProxyChecker;

internal class Program
{
    public static string _path, _type;

    public static ConcurrentQueue<string> _proxy = new();
    public static List<string> _workingProxy = new();

    public static bool _IsRunning = true;
    public static int _success, _fail;

    public static async Task Main(string[] args)
    {
        _type = ConsoleMessage.proxyOptions();

        _path = ConsoleMessage.getPath();

        await ReadyProxyAsync(_path);


        var bots = new Thread[ConsoleMessage.returnThreadNumber()];
        for (int i = 0; i < bots.Length; i++)
        {
            bots[i] = new(() => Worker().Wait());
            bots[i].Start();
        }

        _ = Task.Run(UpdateStatusAsync);

        foreach (var bot in bots)
        {
            bot.Join();
        }

        if (_workingProxy.Count() != 0)
        {
            await WriteWorkingProxyAsync(Directories.WRITING_LOCATION);
        }
        else
        {
            Console.WriteLine("No proxy found");
        }
        ConsoleMessage.Exit();
    }

    public static async Task UpdateStatusAsync()
    {
        while (_IsRunning)
        {
            Console.Title = $"Sucess: {_success} || Fail: {_fail}";
            await Task.Delay(42);
        }
    }

    public static async ValueTask<bool> WriteWorkingProxyAsync(string path)
    {
        Console.WriteLine("Writing working proxy...");
        try
        {
            using StreamWriter writer = File.AppendText(path);
            foreach (var proxy in _workingProxy)
            {
                await writer.WriteLineAsync(proxy);
            }
            return _workingProxy.Count == 0;
        }
        catch
        {
            return false;
        }

    }

    public static async ValueTask<bool> ReadyProxyAsync(string path)
    {
        Console.Title = "Reading proxies...";
        string? line;

            try
            {
                using var reader = new StreamReader(path);
                while (
                    (line = await reader.ReadLineAsync()) != null
                    )
                {
                    _proxy.Enqueue(_type + line);
                }
                Console.Title = "Done reading proxies";
                return !_proxy.IsEmpty;

            }
            catch
            {
                ConsoleMessage.Exit("Failed to read proxies");
                return false;
            }


    }

    public static async Task Worker()
    {
        while (!_proxy.IsEmpty)
        {
            if (!_proxy.TryDequeue(out var proxy))
            {
                Console.WriteLine("fail");
                return;
            }

            try
            {
                var handler = new HttpClientHandler()
                {
                    UseProxy = true,
                    Proxy = new WebProxy(proxy)
                };

                using var httpClient = new HttpClient(handler, true);
                httpClient.Timeout = TimeSpan.FromSeconds(1);

                await httpClient.GetAsync("https://www.google.com/");
                Interlocked.Increment(ref _success);

                _workingProxy.Add(proxy);
            }
            catch
            {
                Interlocked.Increment(ref _fail);
            }
            
        }
    }
}