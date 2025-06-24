using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JustFilter.infrastructure.ai;
using JustFilter.infrastructure.ai.data;
using JustFilter.infrastructure.ai.model;
using JustFilter.infrastructure.datastore.mongo.deleted_messages;
using JustFilter.infrastructure.datastore.redis;
using JustFilter.presentation.printers.common;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;

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
    private readonly IConfiguration _configuration;

    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService interactions,
        CommandService commands,
        IServiceProvider services,
        RedisContext redisContext,
        OllamaHttpClient ollamaHttpClient,
        DeletedMessageRepository deletedMessageRepository,
        IConfiguration configuration
    ) {
        _client = client;
        _interactions = interactions;
        _commands = commands;
        _services = services;
        _redisContext = redisContext;
        _ollamaHttpClient = ollamaHttpClient;
        _deletedMessageRepository = deletedMessageRepository;
        _configuration = configuration;
        
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
        var userId = message.Author.Id;
        var userName = (message.Author as SocketGuildUser)?.GlobalName;

        if (serverId != null)
        {
            var messageText = message.Content;
            var configs = _redisContext.GetConfigsAsync(serverId.Value, channelId).Result;
            var prompt = OllamaConst.Prompt(messageText, configs);

            var ollamaRequest = OllamaGenerateRequest.Default(prompt);
            var result = await _ollamaHttpClient.GenerateAsync(ollamaRequest);

            if (result == null)
            {
                await message.Channel.SendMessageAsync("Something went wrong");
                return;
            }

            if (result.Matches)
            {
                var deletedMessageId = ObjectId.GenerateNewId(DateTime.Now);
                var deletedMessage = new DeletedMessageData
                {
                    Id = deletedMessageId,
                    DeletedMessage = messageText,
                    DeletingReason = result.Reason,
                    AuthorId = userId,
                    AuthorName = userName ?? "Unknown",
                };
                await message.DeleteAsync();
                await _deletedMessageRepository.AddDeletedMessage(deletedMessage);

                var baseUrl = _configuration["JustFilterPanel:BaseUrl"] ?? 
                              throw new InvalidOperationException("JustFilterPanel:BaseUrl is null");
                await message.Channel.SendMessageAsync(
                    embed: InteractionHandlerPrinter.CreateMessageAboutDeletedMessage(),
                    components: InteractionHandlerPrinter.BuildDeletedMessageInfoButton(deletedMessageId, baseUrl)
                );
            }
        }
    }
}