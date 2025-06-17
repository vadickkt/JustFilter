using Discord.Interactions;

namespace JustFilter.commands;

public class UtilityCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("language", "Changes the language of the bot")]
    public async Task PingAsync()
    {
        await RespondAsync($"Not implemented yet.");
    }
}