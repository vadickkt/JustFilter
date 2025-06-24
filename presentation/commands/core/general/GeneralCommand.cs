using Discord.Interactions;
using JustFilter.presentation.commands.core.core;
using JustFilter.presentation.printers.general;

namespace JustFilter.presentation.commands.core.general;

public class GeneralCommand : BaseCommandModule
{
    [SlashCommand("ping", "Check bot latency")]
    public async Task PingAsync()
    {
        var latency = Context.Client.Latency;
        await RespondAsync(embed: PingCommandPrinter.CreateRespondWithLatency(latency), ephemeral: true);
    }

    [SlashCommand("help", "Show help menu")]
    public async Task HelpAsync()
    {
        await RespondAsync(embed: HelpPrinter.BuildHelpMessage(), ephemeral: true);
    }
}