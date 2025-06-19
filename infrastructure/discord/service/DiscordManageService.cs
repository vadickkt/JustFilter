using Discord;
using Discord.WebSocket;
using JustFilter.infrastructure.discord.handler.core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace JustFilter.infrastructure.discord.service;

public class DiscordManageService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _config;
    private readonly InteractionHandler _interactionHandler;
    private readonly GuildEventHandler _guildEventHandler;

    public DiscordManageService(
        DiscordSocketClient client, 
        IConfiguration config, 
        InteractionHandler interactionHandler,
        GuildEventHandler guildEventHandler
    ) {
        _client = client;
        _config = config;
        _interactionHandler = interactionHandler;
        _guildEventHandler = guildEventHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _interactionHandler.InitializeAsync();
        await _guildEventHandler.InitializeAsync();

        var token = _config["Discord:Token"];
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
    }
}