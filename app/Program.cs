using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JustFilter.services;
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
        var config = new DiscordSocketConfig
        {
            MessageCacheSize = 200,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        services.AddSingleton(new DiscordSocketClient(config));
        services.AddSingleton<LoggingService>();
        services.AddSingleton<CommandService>();
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<DiscordSocketClient>();
            return new InteractionService(client);
        });
        services.AddHostedService<DiscordStartupService>();
    })
    .Build();

await host.RunAsync();