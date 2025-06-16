using Discord.Interactions;

namespace JustFilter.commands;

public class HelpCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("help", "Shows help message")]
    public async Task Help()
    {
        await RespondAsync("Do u need help?");
    }
}