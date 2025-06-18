using Discord;
using Discord.Interactions;
using JustFilter.data.entities;
using JustFilter.infrastructure.database.mongo.repository;
using JustFilter.presentation.printers;

namespace JustFilter.presentation.commands.core;

public class ConfigurationCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public ConfigurationCommand(ConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    [SlashCommand("config-add", "Add a new config")]
    public async Task AddConfigAsync()
    {
        var modal = new ModalBuilder()
            .WithTitle("Add Config")
            .WithCustomId("new_config")
            .AddTextInput("Config Name", "config_name", placeholder: "Example: Religion")
            .AddTextInput("Config Description", "config_description", TextInputStyle.Paragraph, 
                placeholder: "Filter messages about religion");

        await RespondWithModalAsync(modal.Build());
    }

    [SlashCommand("config-delete", "Remove a config")]
    public async Task DeleteConfigAsync([Summary(description: "config name")] string configName)
    {
        var guild = Context.Guild;
        var deleteConfigData = new ChangeConfigData
        {
            DiscordId = guild.Id,
            Name = configName
        };
        var result = await _configRepository.DeleteConfig(deleteConfigData);
        if (result == DeletionResult.Deleted)
        {
            await RespondAsync($"Config {deleteConfigData.Name} has been deleted");
        }
        else await RespondAsync($"Config {deleteConfigData.Name} was not found and has not been deleted.");
    }
    
    [SlashCommand("config-list", "Get list of all configs")]
    public async Task GetAllConfigsAsync()
    {
        var guild = Context.Guild;
        var result = await _configRepository.GetAllConfigs(guild.Id);
        if (result == null) await RespondAsync("No configs found");
        else await RespondAsync(embed: ConfigPrinter.PrintConfigs(result));
    }

    [SlashCommand("config-update", "Update a config")]
    public async Task UpdateConfigAsync([Summary(description: "config name")] string configName)
    {
        var configs = _configRepository.GetAllConfigs(Context.Guild.Id);
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select an option")
            .WithCustomId("menu-1")
            .WithMinValues(1)
            .WithMaxValues(1)
            .AddOption("Option A", "opt-a", "Option B is lying!")
            .AddOption("Option B", "opt-b", "Option A is telling the truth!");
        var builder = new ComponentBuilder()
            .WithSelectMenu(menuBuilder);
            
        await RespondAsync(embed: ConfigPrinter.PrintUpdateMessage(), components: builder.Build());
    }
}