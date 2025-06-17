using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JustFilter.services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace JustFilter.Services;

public class DiscordStartupService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _config;
    private readonly InteractionHandler _interactionHandler;

    public DiscordStartupService(DiscordSocketClient client, IConfiguration config, InteractionHandler interactionHandler)
    {
        _client = client;
        _config = config;
        _interactionHandler = interactionHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _interactionHandler.InitializeAsync();

        var token = _config["Discord:Token"];
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
    }
}