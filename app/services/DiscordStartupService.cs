using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace JustFilter.app.services;

public class DiscordStartupService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;

    public DiscordStartupService(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var token = _configuration["Discord:Token"];
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.LogoutAsync();
        await _client.StopAsync();
    }
}