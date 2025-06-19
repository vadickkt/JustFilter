using Discord.Interactions;
using JustFilter.infrastructure.database.mongo.entities;
using JustFilter.infrastructure.database.mongo.repository;
using JustFilter.presentation.commands.entities;
using MongoDB.Bson;

namespace JustFilter.infrastructure.discord.handler;

public class ConfigModalHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public ConfigModalHandler(ConfigRepository configRepository)
    {
        _configRepository = configRepository;
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
        await RespondAsync($"Config {configId} has been updated.");
    }
}
