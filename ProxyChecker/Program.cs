using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;

namespace ProxyChecker;

internal class Program
{
    public static string _path, _type;

    public static ConcurrentQueue<string> _proxy = new();
    public static BlockingCollection<string> _workingProxy = new();

    public static bool _IsRunning = true;
    public static int _success, _fail;

    public static async Task Main(string[] args)
    {
        _type = ConsoleMessage.proxyOptions();

        _path = ConsoleMessage.getPath();

        await ReadProxyAsync(_path);


        var bots = new Thread[ConsoleMessage.returnThreadNumber()];

        for (int i = 0; i < bots.Length; i++)
        {
            bots[i] = new(() => Worker().Wait());
            bots[i].Start();
        }

        _ = Task.Run(UpdateStatusAsync);
        await WriteWorkingProxyInStreamAsync(Directories.WRITING_LOCATION);

        foreach (var bot in bots)
        {
            bot.Join();
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

    public static async Task WriteWorkingProxyInStreamAsync(string path)
    {
        try
        {
            using StreamWriter writer = File.AppendText(path);

            foreach (var item in _workingProxy.GetConsumingEnumerable())
            {
                await writer.WriteLineAsync(item);
            }
        }
        catch
        {
            ConsoleMessage.Exit("Failed to write proxies");
        }
    }

    public static async ValueTask<bool> ReadProxyAsync(string path)
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

        _workingProxy.CompleteAdding();
    }
}