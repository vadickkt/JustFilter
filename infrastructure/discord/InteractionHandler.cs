using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace JustFilter.infrastructure.discord;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;

    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService interactions,
        CommandService commands,
        IServiceProvider services)
    {
        _client = client;
        _interactions = interactions;
        _commands = commands;
        _services = services;

        _client.Ready += OnReady;
        _client.InteractionCreated += HandleInteraction;
        _client.MessageReceived += HandleMessageAsync;
    }

    public async Task InitializeAsync()
    {
        await _interactions.AddModulesAsync(typeof(InteractionHandler).Assembly, _services);
    }

    private async Task OnReady()
    {
        await _interactions.RegisterCommandsGloballyAsync();
        Console.WriteLine("Slash-commands registered");
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(_client, interaction);
        await _interactions.ExecuteCommandAsync(context, _services);
    }

    private async Task HandleMessageAsync(SocketMessage message)
    {
        if (message is not SocketUserMessage userMessage || message.Author.IsBot) return;

        var argPos = 0;
        if (!userMessage.HasCharPrefix('!', ref argPos)) return;

        var context = new SocketCommandContext(_client, userMessage);
        var result = await _commands.ExecuteAsync(context, argPos, _services);
        
        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            await context.Channel.SendMessageAsync($"Failure: {result.ErrorReason}");
    }
}
