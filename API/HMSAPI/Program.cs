using HMSAPI;
using NLog.Web;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders(); // Remove default loggers
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            })
            .UseNLog() // Enable NLog
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}