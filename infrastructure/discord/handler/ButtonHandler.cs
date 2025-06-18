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
        await RespondAsync("create_config_button");
    }

    [ComponentInteraction("delete_config_button")]
    public async Task DeleteConfigButton()
    {
        await RespondAsync("delete_config_button");
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