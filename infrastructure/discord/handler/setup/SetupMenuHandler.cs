using Discord.Interactions;
using JustFilter.infrastructure.datastore.mongo.channel;
using JustFilter.infrastructure.datastore.redis;
using MongoDB.Bson;

namespace JustFilter.infrastructure.discord.handler.setup;

public class SetupMenuHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ChannelRepository _channelRepository;
    private readonly RedisContext _redisContext;

    public SetupMenuHandler(ChannelRepository channelRepository, RedisContext redisContext)
    {
        _channelRepository = channelRepository;
        _redisContext = redisContext;
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
            await _redisContext.UpdateConfigsAsync(guild.Id, channel.Id, selectedConfigs.ToList());
            await RespondAsync("Channel configurations have been updated.");
        }
        else
        {
            await _redisContext.AddConfigsAsync(guild.Id, channel.Id, selectedConfigs.ToList());
            await RespondAsync("Channel configurations have been added.");
        }
    }
}