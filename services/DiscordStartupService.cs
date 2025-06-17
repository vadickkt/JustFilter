using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
using JustFilter.commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace JustFilter.services;

public class DiscordStartupService : IHostedService {
    
    private readonly DiscordSocketClient client;
    private readonly IConfiguration configuration;
    private readonly InteractionService interactionService;
    private readonly CommandService commands;

    public DiscordStartupService(
        DiscordSocketClient client,
        IConfiguration configuration,
        InteractionService interactionService,
        CommandService commands
    )
    {
        this.client = client;
        this.configuration = configuration;
        this.interactionService = interactionService;
        this.commands = commands;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var token = configuration["Discord:Token"];
        
        await interactionService.AddModulesAsync(typeof(GeneralCommand).Assembly, null);
        
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
        
        client.MessageReceived += MessageReceived;
        client.InteractionCreated += HandleInteraction;
        client.Ready += ReadyAsync;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.LogoutAsync();
        await client.StopAsync();
    }

    private async Task ReadyAsync()
    {
        await interactionService.RegisterCommandsGloballyAsync();
        Console.WriteLine("Slash commands registered.");
    }


    private async Task HandleInteraction(SocketInteraction interaction)
    {
        var ctx = new SocketInteractionContext(client, interaction);
        await interactionService.ExecuteCommandAsync(ctx, null);
    }
    
    private async Task MessageReceived(SocketMessage message)
    {
        if (message is not SocketUserMessage userMessage || message.Author.IsBot || message.Author.IsWebhook)
            return;

        int argPos = 0;
        char prefix = '!';

        if (!(userMessage.HasCharPrefix(prefix, ref argPos) || userMessage.HasMentionPrefix(client.CurrentUser, ref argPos)))
            return;

        var context = new SocketCommandContext(client, userMessage);
        var result = await commands.ExecuteAsync(context, argPos, services: null);
        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
        {
            await context.Channel.SendMessageAsync($"❌ Fehler: {result.ErrorReason}");
        }
    }
}