using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JustFilter.infrastructure.discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

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
        
        var dbConnectionString = context.Configuration.GetConnectionString("MongoDB:ConnectionString");
        services.AddSingleton<IMongoClient>(new MongoClient(dbConnectionString));
    })
    .Build();

await host.RunAsync();