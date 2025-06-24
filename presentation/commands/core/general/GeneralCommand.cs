using Discord.Interactions;
using JustFilter.presentation.printers.general;

namespace JustFilter.presentation.commands.core.general;

public class GeneralCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Check bot latency")]
    public async Task PingAsync()
    {
        await RespondAsync($"🏓 Pong! Latency: {Context.Client.Latency}ms", ephemeral: true);
    }

    [SlashCommand("help", "Show help menu")]
    public async Task HelpAsync()
    {
        await RespondAsync(embed: HelpPrinter.BuildHelpMessage(), ephemeral: true);
    }
}