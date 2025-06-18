using Discord;
using Discord.Interactions;
using JustFilter.data.utils;
using JustFilter.infrastructure.database.mongo.repository;
using JustFilter.presentation.printers;

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
        // TODO if else do something
    }
}