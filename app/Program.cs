using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JustFilter.services;
using JustFilter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var discordConfig = new DiscordSocketConfig
        {
            MessageCacheSize = 200,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        services.AddSingleton(new DiscordSocketClient(discordConfig));
        services.AddSingleton<CommandService>();
        
        services.AddSingleton<InteractionService>(sp =>
        {
            var client = sp.GetRequiredService<DiscordSocketClient>();
            return new InteractionService(client);
        });

        services.AddSingleton<LoggingService>();
        services.AddSingleton<InteractionHandler>();
        services.AddHostedService<DiscordStartupService>();
    })
    .Build();

await host.RunAsync();