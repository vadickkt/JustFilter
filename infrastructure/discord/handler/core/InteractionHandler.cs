using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace JustFilter.infrastructure.discord.handler.core;

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
        IServiceProvider services
    ) {
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
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(_client, interaction);
        await _interactions.ExecuteCommandAsync(context, _services);
    }

    private async Task HandleMessageAsync(SocketMessage message)
    {
        if (message is not SocketUserMessage userMessage || message.Author.IsBot) return;

        await HandleMessage(message);

        var argPos = 0;
        if (!userMessage.HasCharPrefix('!', ref argPos)) return;

        var context = new SocketCommandContext(_client, userMessage);
        var result = await _commands.ExecuteAsync(context, argPos, _services);

        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            await context.Channel.SendMessageAsync($"Failure: {result.ErrorReason}");
    }

    private async Task HandleMessage(SocketMessage message)
    {
        var serverId = (message.Channel as SocketGuildChannel)?.Guild.Id;
        var channelId = message.Channel.Id;

        if (serverId != null)
        {
            var messageText = message.Content;
            // TODO redis cache confis
        }
        
        Console.WriteLine($"[{message.Author.Username} in #{message.Channel.Name}]: {message.Content}");
        Console.WriteLine($"serverId: {serverId},  channelId: {channelId}");
        
    }
    
    /*

var httpClient = new HttpClient();
     const string baseUrl = "http://localhost:11434/api/";
     var ollamaHttpClient = new OllamaHttpClient(httpClient, baseUrl);
     var ollamaRequest = new OllamaGenerateRequest{
         model = "deepseek-r1:latest",
         prompt = OllamaConst.Prompt("Украина лучше России", configs),
         stream = false,
         format = new FormatRequest{
             type = "object",
             properties = new Dictionary<string, FormatProperty>{{ "isPolicy", new FormatProperty { type = "boolean" } },{ "explanation", new FormatProperty { type = "string" } }},
             required = ["isPolicy", "explanation"]}};
     var result = await ollamaHttpClient.GenerateAsync(ollamaRequest);
     Console.WriteLine(result);
     await RespondAsync(result);*/
}
