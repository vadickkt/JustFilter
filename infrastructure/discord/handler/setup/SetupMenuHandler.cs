using Discord.Interactions;
using JustFilter.infrastructure.database.mongo.config;

namespace JustFilter.infrastructure.discord.handler.setup;

public class SetupMenuHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public SetupMenuHandler(ConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }
    
    [ComponentInteraction("setup_menu")]
    public async Task HandleSetupMenuAsync(string[] selectedConfigs)
    {
        // TODO send it to ai and handle then
        await RespondAsync($"selectedConfigs: {string.Join(",", selectedConfigs)}");
    }
}