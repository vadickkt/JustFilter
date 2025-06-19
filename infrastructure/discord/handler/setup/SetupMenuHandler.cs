using Discord.Interactions;
using JustFilter.infrastructure.ai;
using JustFilter.infrastructure.ai.data;
using JustFilter.infrastructure.ai.model;
using JustFilter.infrastructure.datastore.mongo.channel;
using JustFilter.infrastructure.datastore.mongo.config;
using MongoDB.Bson;
using OllamaSharp;
using OllamaSharp.Models;

namespace JustFilter.infrastructure.discord.handler.setup;

public class SetupMenuHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;
    private readonly ChannelRepository _channelRepository;

    public SetupMenuHandler(ConfigRepository configRepository, ChannelRepository channelRepository)
    {
        _configRepository = configRepository;
        _channelRepository = channelRepository;
    }

    [ComponentInteraction("setup_menu")]
    public async Task HandleSetupMenuAsync(string[] selectedConfigs)
    {
        var guild = Context.Guild;
        var channel = Context.Channel;

        if (selectedConfigs.Length == 0)
        {
            await RespondAsync("No configurations selected.");
            return;
        }

        var configIds = selectedConfigs
            .Select(ObjectId.Parse)
            .ToList();

        var channelData = new ChannelData
        {
            ChannelId = channel.Id,
            ServerId = guild.Id,
            Name = channel.Name,
            ConfigsIds = configIds
        };

        var result = await _channelRepository.AddChannelIfNotExistAsync(channelData);

        if (result.Existed)
        {
            result.ChannelData.ConfigsIds = configIds;
            await _channelRepository.UpdateChannel(result.ChannelData);
            await RespondAsync("Channel configurations have been updated.");
        }
        else
        {
            await RespondAsync("Channel configurations have been added.");
        }
    }
}