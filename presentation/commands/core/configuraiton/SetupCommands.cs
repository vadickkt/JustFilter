using Discord.Interactions;
using JustFilter.infrastructure.database.mongo.config;
using JustFilter.presentation.printers;
using JustFilter.presentation.printers.setup;

namespace JustFilter.presentation.commands.core.configuraiton;

public class SetupCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public SetupCommands(ConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }
    
    [SlashCommand("setup", "Setup JustFilter in a channel")]
    public async Task SetupAsync()
    {
        var configs = await _configRepository.GetAllConfigs(Context.Guild.Id);
        if (configs != null)
        {
            await RespondAsync(
                embed: SetupPrinter.BuildListOfAvailableConfigs(configs), 
                components: SetupPrinter.BuildSetupComponents(configs)
            );
        }
        else
        {
            await RespondAsync("dasdsadas");
        }
        // TODO if else do something
    }
}