using System.Text.Json;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JustFilter.infrastructure.ai;
using JustFilter.infrastructure.ai.data;
using JustFilter.infrastructure.ai.model;
using JustFilter.infrastructure.datastore.mongo.deleted_messages;
using JustFilter.infrastructure.datastore.redis;

namespace JustFilter.infrastructure.discord.handler.core;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly RedisContext _redisContext;
    private readonly OllamaHttpClient _ollamaHttpClient;
    private readonly DeletedMessageRepository _deletedMessageRepository;

    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService interactions,
        CommandService commands,
        IServiceProvider services,
        RedisContext redisContext,
        OllamaHttpClient ollamaHttpClient,
        DeletedMessageRepository deletedMessageRepository
    ) {
        _client = client;
        _interactions = interactions;
        _commands = commands;
        _services = services;
        _redisContext = redisContext;
        _ollamaHttpClient = ollamaHttpClient;
        _deletedMessageRepository = deletedMessageRepository;

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
            var configs = _redisContext.GetConfigsAsync(serverId.Value, channelId).Result;
            var prompt = OllamaConst.Prompt(messageText, configs);

            var ollamaRequest = new OllamaGenerateRequest
            {
                model = "deepseek-r1:latest",
                prompt = prompt,
                stream = false,
                format = new FormatRequest
                {
                    type = "object",
                    properties = new Dictionary<string, FormatProperty>
                    {
                        { "matches", new FormatProperty { type = "boolean" } },
                        { "category", new FormatProperty { type = new[] { "string", "null" } } },
                        { "reason", new FormatProperty { type = new[] { "string", "null" } } }
                    },
                    required = ["matches", "category", "reason"]
                }
            };
            var result = await _ollamaHttpClient.GenerateAsync(ollamaRequest);

            if (result == null)
            {
                await message.Channel.SendMessageAsync("Something went wrong");
                return;
            }

            if (result.Matches)
            {
                var deletedMessage = new DeletedMessageData
                {
                    DeletedMessage = messageText,
                    DeletingReason = result.Reason
                };
                await message.DeleteAsync();
                await _deletedMessageRepository.AddDeletedMessage(deletedMessage);
            }
        }
    }
}