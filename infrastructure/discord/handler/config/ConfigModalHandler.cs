using Discord.Interactions;
using JustFilter.infrastructure.datastore.mongo.config;
using JustFilter.infrastructure.datastore.redis;
using JustFilter.presentation.commands.entities.config;
using MongoDB.Bson;

namespace JustFilter.infrastructure.discord.handler.config;

public class ConfigModalHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;
    private readonly RedisContext  _redisContext;

    public ConfigModalHandler(ConfigRepository configRepository, RedisContext redisContext)
    {
        _configRepository = configRepository;
        _redisContext = redisContext;
    }

    [ModalInteraction("new_config")]
    public async Task HandleAddConfigModalAsync(AddConfigModal modal)
    {
        var guild = Context.Guild;

        if (guild != null)
        {
            var configData = new ConfigData
            {
                DiscordId = guild.Id,
                Name = modal.ConfigName,
                Description = modal.ConfigDescription,
            };
            await _configRepository.AddConfig(configData);
            await RespondAsync($"Config {configData.Name} has been saved.");
        }
    }

    [ModalInteraction("new_config_name_modal:*")]
    public async Task HandleEditConfigNameModal(string configId, EditConfigNameModal modal)
    {
        var parsedConfigId = ObjectId.Parse(configId);
        var oldConfig = await _configRepository.GetConfigById(parsedConfigId);
        oldConfig.Name = modal.NewConfigName;
        await _configRepository.UpdateConfig(parsedConfigId, oldConfig);
        await _redisContext.UpdateConfigAsync(Context.Guild.Id, Context.Channel.Id, oldConfig);
        await RespondAsync($"Config {configId} has been updated.");
    }

    [ModalInteraction("new_config_description_modal:*")]
    public async Task HandleEditConfigNameModal(string configId, EditConfigDescriptionModal modal)
    {
        var parsedConfigId = ObjectId.Parse(configId);
        var oldConfig = await _configRepository.GetConfigById(parsedConfigId);
        oldConfig.Description = modal.NewConfigDescription;
        await _configRepository.UpdateConfig(parsedConfigId, oldConfig);
        await _redisContext.UpdateConfigAsync(Context.Guild.Id, Context.Channel.Id, oldConfig);
        await RespondAsync($"Config {configId} has been updated.");
    }
}
