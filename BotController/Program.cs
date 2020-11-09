using System;
using System.IO;
using FileReceiverBot;
using FileReceiverBot.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;

namespace BotController
{
    class Program
    {
        private static void Main()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .CreateLogger();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IFileReceiverBotClient>(new FileReceiverBotClient(builder.Build()["Bots:FileReceiver:Token"]));
                    services.AddSingleton<IFileReceiverBot, FileReceiverBot.FileReceiverBot>();
                })
                .UseSerilog()
                .Build();


            var bot = ActivatorUtilities.CreateInstance<FileReceiverBot.FileReceiverBot>(host.Services);
            bot.Execute();
        }

        private static void BuildConfig(ConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
