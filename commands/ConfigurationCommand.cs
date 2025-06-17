using Discord;
using Discord.Interactions;
using JustFilter.infrastructure.database.mongo.repository;

namespace JustFilter.commands;

public class ConfigurationModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public ConfigurationModule(ConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }
    
    [SlashCommand("config-add", "Add a new configuration")]
    public async Task AddConfigAsync()
    {
        var modal = new ModalBuilder()
            .WithTitle("Add Config")
            .WithCustomId("new_config")
            .AddTextInput("Config Name", "config_name", placeholder: "Example: Religion")
            .AddTextInput("Config Description", "config_description", TextInputStyle.Paragraph, placeholder: "Filter messages about religion");

        await RespondWithModalAsync(modal.Build());
    }
}