using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Logging;

using OpenTelemetry.Trace;

namespace otel_test_1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;

            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logBuilder =>
                    logBuilder.ClearProviders()
                              .AddOpenTelemetry(options =>
                                options.AddConsoleExporter()
                ))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
