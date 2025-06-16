using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace JustFilter.services;

public class DiscordStartupService(DiscordSocketClient client, CommandService commands, IConfiguration configuration) : IHostedService {
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var token = configuration["Discord:Token"];
        
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        client.MessageReceived += MessageReceived;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.LogoutAsync();
        await client.StopAsync();
    }

    private static async Task MessageReceived(SocketMessage message)
    {
        if (message.Author.IsBot || message.Author.IsWebhook) return;
        
        await message.Channel.SendMessageAsync(message.Content);
    }
}