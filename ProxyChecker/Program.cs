using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Net;
using System.Net.Http.Headers;

namespace ProxyChecker;

internal class Program
{
    public static ConcurrentQueue<string> _proxy = new();
    public static BlockingCollection<string> _workingProxy = new();

    public static bool _IsRunning = true;
    public static int _success, _fail;

    public static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            ConfigRunOptions.PrintUsage();
            ConfigRunOptions.Exit();
        }
        await ConfigRunOptions.CommandLineOptions(args);

        await Application();
    }

    public static async Task Application()
    {
        if (ConfigRunOptions._isFile)
            await ReadProxyAsync(ConfigRunOptions._sourceToRead);
        else
            await ReadProxyFromUrl(ConfigRunOptions._sourceToRead);

        var bots = new Thread[ConfigRunOptions._threads];

        for (int i = 0; i < bots.Length; i++)
        {
            bots[i] = new(() => Worker().Wait());
            bots[i].Start();
        }

        _ = Task.Run(UpdateStatusAsync);

        await WriteWorkingProxyInStreamAsync(ConfigRunOptions._pathToWrite);

        foreach (var bot in bots)
        {
            bot.Join();
        }

        ConfigRunOptions.Exit();
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
            ConfigRunOptions.Exit("Failed to write proxies");
        }
    }

    public static async Task<bool> ReadProxyAsync(string path)
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
                _proxy.Enqueue(ConfigRunOptions._proxyType + line);
            }
            Console.Title = "Done reading proxies";
            return !_proxy.IsEmpty;
        }
        catch
        {
            ConfigRunOptions.Exit("Failed to read proxies");
            return false;
        }
    }

    public static async Task<bool> ReadProxyFromUrl(string url)
    {
        try
        {
            var client = new HttpClient();
            var res = await client.GetAsync(ConfigRunOptions._sourceToRead);
            var mediaType = res.Content.Headers.ContentType;
            if (mediaType != null &&
                !mediaType.MediaType.StartsWith("text/"))
            {
                ConfigRunOptions.Exit("URL is not text");
            }
            foreach (var proxy in 
                (await res.Content.ReadAsStringAsync()).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                )
            {
                _proxy.Enqueue(proxy);
            }
            Console.Title = "Done reading proxies";
            return !_proxy.IsEmpty;
        }
        catch { return false; }
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