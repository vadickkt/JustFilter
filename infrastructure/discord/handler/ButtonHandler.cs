using Discord;
using Discord.Interactions;
using JustFilter.infrastructure.database.mongo.repository;
using JustFilter.presentation.printers;

namespace JustFilter.infrastructure.discord.handler;

public class ButtonHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public ButtonHandler(ConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }
    
    [ComponentInteraction("create_config_button")]
    public async Task CreateConfigButton()
    {
        var modal = new ModalBuilder()
            .WithTitle("Add Config")
            .WithCustomId("new_config")
            .AddTextInput("Config Name", "config_name", placeholder: "Example: Religion")
            .AddTextInput("Config Description", "config_description", TextInputStyle.Paragraph,
                placeholder: "Filter messages about religion");

        await RespondWithModalAsync(modal.Build());
    }

    [ComponentInteraction("delete_config_button")]
    public async Task DeleteConfigButton()
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
    
    [ComponentInteraction("edit_config_button")]
    public async Task EditConfigButton()
    {
        var configs = await _configRepository.GetAllConfigs(Context.Guild.Id);
        if (configs != null)
        {
            await RespondAsync(
                embed: EditConfigPrinter.PrintUpdateMessage(),
                components: EditConfigPrinter.BuildConfigEditComponents(configs)
            );
        }
    }
}