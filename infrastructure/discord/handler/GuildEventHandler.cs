using Discord.Commands;

namespace JustFilter.infrastructure.discord.handler;

using Discord.WebSocket;
using System;
using System.Threading.Tasks;

public class GuildEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;

    public GuildEventHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
    {
        _client = client;
        _commands = commands;
        _services = services;

        _client.JoinedGuild += OnJoinedGuildAsync;
    }
    
    public async Task InitializeAsync()
    {
        await _commands.AddModulesAsync(typeof(InteractionHandler).Assembly, _services);
    }
    
    private Task OnJoinedGuildAsync(SocketGuild guild)
    {
        Console.WriteLine($"Bot was added to server {guild.Name} (ID: {guild.Id})");
        return Task.CompletedTask;
    }
}
