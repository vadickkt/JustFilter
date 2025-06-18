using Discord.Interactions;
using JustFilter.infrastructure.database.mongo.entities;
using JustFilter.infrastructure.database.mongo.repository;
using JustFilter.presentation.commands.entities;

namespace JustFilter.infrastructure.discord.handler;

public class ConfigurationCommandHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public ConfigurationCommandHandler(ConfigRepository configRepository)
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

}
