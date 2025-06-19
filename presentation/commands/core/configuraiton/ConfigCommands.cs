using Discord;
using Discord.Interactions;
using JustFilter.infrastructure.database.mongo.config;
using JustFilter.presentation.printers;

namespace JustFilter.presentation.commands.core.configuraiton;

public class ConfigCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public ConfigCommands(ConfigRepository configRepository)
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
    public async Task DeleteConfigAsync()
    {
        var configs = await _configRepository.GetAllConfigs(Context.Guild.Id);
        if (configs != null)
        {
            await RespondAsync(
                embed: DeleteConfigPrinter.PrintDeleteMessage(), 
                components: DeleteConfigPrinter.BuildConfigDeleteComponents(configs)
            );
        }
    }

    [SlashCommand("config-list", "Get list of all configs")]
    public async Task GetAllConfigsAsync()
    {
        var guild = Context.Guild;
        var result = await _configRepository.GetAllConfigs(guild.Id);
        if (result == null) await RespondAsync("No configs found");
        else await RespondAsync(embed: GetAllConfigsPrinter.PrintConfigs(result));
    }

    [SlashCommand("config-update", "Update a config")]
    public async Task UpdateConfigAsync()
    {
        var configs = await _configRepository.GetAllConfigs(Context.Guild.Id);
        if (configs != null)
        {
            await RespondAsync(
                embed: EditConfigPrinter.PrintStartUpdateMessage(), 
                components: EditConfigPrinter.BuildStartConfigEditComponents(configs)
            );
        }
    }
}