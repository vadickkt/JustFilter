using Discord.Interactions;
using JustFilter.infrastructure.datastore.mongo.channel;
using JustFilter.infrastructure.datastore.mongo.config;
using JustFilter.infrastructure.datastore.redis;
using JustFilter.presentation.printers.config;
using MongoDB.Bson;

namespace JustFilter.infrastructure.discord.handler.config;

public class ConfigMenuHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;
    private readonly ChannelRepository _channelRepository;
    private readonly RedisContext _redisContext;

    public ConfigMenuHandler(ConfigRepository configRepository, RedisContext redisContext, ChannelRepository channelRepository)
    {
        _configRepository = configRepository;
        _redisContext = redisContext;
        _channelRepository = channelRepository;
    }
    
    [ComponentInteraction("delete_config_menu")]
    public async Task HandleDeleteConfigMenuAsync(string[] selectedConfig)
    {
        foreach (var configId in selectedConfig)
        {
            var objectId = ObjectId.Parse(configId);
            await _configRepository.DeleteConfigByObjectId(objectId);
            await _channelRepository.DeleteConfigInChannel(Context.Guild.Id, Context.Channel.Id, objectId);
            await _redisContext.RemoveConfigAsync(Context.Guild.Id, Context.Channel.Id, objectId);
        }
        await RespondAsync("These configs have been successfully deleted");
    }
    
    [ComponentInteraction("edit_config_menu")]
    public async Task HandleUpdateConfigMenuAsync(string[] selectedConfig)
    {
        var configId = ObjectId.Parse(selectedConfig[0]);
        var config = await _configRepository.GetConfigById(configId);

        await RespondAsync(
            embed: EditConfigPrinter.PrintFinalUpdateMessage(config), 
            components: EditConfigPrinter.BuildFinalConfigEditComponents(config)
        );
    }
}