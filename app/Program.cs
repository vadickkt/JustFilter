using Discord.WebSocket;
using JustFilter.app.services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<LoggingService>();
        services.AddHostedService<DiscordStartupService>();
    })
    .Build();

await host.RunAsync();