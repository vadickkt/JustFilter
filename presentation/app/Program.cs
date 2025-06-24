using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JustFilter.infrastructure.ai;
using JustFilter.infrastructure.datastore.mongo;
using JustFilter.infrastructure.datastore.mongo.channel;
using JustFilter.infrastructure.datastore.mongo.config;
using JustFilter.infrastructure.datastore.mongo.deleted_messages;
using JustFilter.infrastructure.datastore.mongo.server;
using JustFilter.infrastructure.datastore.redis;
using JustFilter.infrastructure.discord.handler.core;
using JustFilter.infrastructure.discord.service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using StackExchange.Redis;

var host = Host.CreateDefaultBuilder(args)
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

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisBaseUrl = configuration["Redis:ConnectionString"] ?? "localhost:6379,abortConnect=false"; 
            var redisConfiguration = ConfigurationOptions.Parse(redisBaseUrl, true);
            return ConnectionMultiplexer.Connect(redisConfiguration);
        });
        services.AddSingleton<RedisContext>();
        services.AddSingleton<HttpClient>();
        services.AddSingleton<OllamaHttpClient>();
        services.AddSingleton<DeletedMessageRepository>();
    }).Build();

await host.RunAsync();