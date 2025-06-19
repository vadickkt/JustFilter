using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JustFilter.infrastructure.database.mongo;
using JustFilter.infrastructure.database.mongo.server;
using JustFilter.infrastructure.datastore.mongo;
using JustFilter.infrastructure.datastore.mongo.channel;
using JustFilter.infrastructure.datastore.mongo.config;
using JustFilter.infrastructure.discord.handler.core;
using JustFilter.infrastructure.discord.service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using StackExchange.Redis;

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
        services.AddHostedService<DiscordManageService>();
        
        services.AddSingleton<IMongoClient>(new MongoClient(dbConnectionString));
        services.AddSingleton<MongoDbContext>();
        services.AddSingleton<DiscordServersRepository>();
        services.AddSingleton<ConfigRepository>();
        services.AddSingleton<ChannelRepository>();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisBaseUrl = configuration["Redis:BaseUrl"] ?? "http://localhost:6379"; 
            var redisConfiguration = ConfigurationOptions.Parse(redisBaseUrl, true);
            return ConnectionMultiplexer.Connect(redisConfiguration);
        });
    }).Build();

await host.RunAsync();