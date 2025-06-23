using Discord.Commands;
using Discord.WebSocket;
using JustFilter.infrastructure.database.mongo.server;
using JustFilter.infrastructure.datastore.mongo.server;

namespace JustFilter.infrastructure.discord.handler.core;

public class GuildEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly DiscordServersRepository _serversRepository;

    public GuildEventHandler(
        DiscordSocketClient client, 
        CommandService commands,
        IServiceProvider services,
        DiscordServersRepository serversRepository
    ) {
        _client = client;
        _commands = commands;
        _services = services;
        _serversRepository = serversRepository;
        
        _client.JoinedGuild += OnJoinedGuildAsync;
        _client.LeftGuild += OnLeftGuildAsync;
    }
    
    public async Task InitializeAsync()
    {
        await _commands.AddModulesAsync(typeof(InteractionHandler).Assembly, _services);
    }
    
    private Task OnJoinedGuildAsync(SocketGuild guild)
    {
        var discordServer = new ServerData
        {
            Name = guild.Name,
            ServerId = guild.Id
        };
        _serversRepository.AddServer(discordServer);
        return Task.CompletedTask;
    }

    private Task OnLeftGuildAsync(SocketGuild guild)
    {
        _serversRepository.DeleteServerByServerId(guild.Id);
        return Task.CompletedTask;
    }
}
