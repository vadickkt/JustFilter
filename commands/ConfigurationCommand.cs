using Discord;
using Discord.Interactions;

namespace JustFilter.commands;

public class ConfigurationModule : InteractionModuleBase<SocketInteractionContext>
{
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