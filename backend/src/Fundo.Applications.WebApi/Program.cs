using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using System;

namespace Fundo.Applications.WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Serilog bootstrap
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter())
                .CreateLogger();

            // Optional: Loki sink if LOKI_URL is set
            var lokiUrl = configuration["LOKI_URL"];
            if (!string.IsNullOrWhiteSpace(lokiUrl))
            {
                var appName = configuration["APP_NAME"] ?? "loan-api";
                var environmentName = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("app", appName)
                    .Enrich.WithProperty("environment", environmentName)
                    .WriteTo.Console(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter())
                    .WriteTo.GrafanaLoki(lokiUrl)
                    .CreateLogger();
            }

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Unhandled WebApi exception");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
