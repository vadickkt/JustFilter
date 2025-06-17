using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JustFilter.infrastructure.database.mongo;
using JustFilter.infrastructure.database.mongo.repository;
using JustFilter.infrastructure.discord.handler;
using JustFilter.infrastructure.discord.service;
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
        var configuration = context.Configuration;
        var dbConnectionString = configuration["MongoDB:ConnectionString"];
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
        services.AddSingleton<GuildEventHandler>();
        services.AddHostedService<DiscordStartupService>();
        
        services.AddSingleton<IMongoClient>(new MongoClient(dbConnectionString));
        services.AddSingleton<MongoDbContext>();
        services.AddSingleton<DiscordServersRepository>();
    })
    .Build();

await host.RunAsync();