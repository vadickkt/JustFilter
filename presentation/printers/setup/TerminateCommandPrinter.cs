using Discord;

namespace JustFilter.presentation.printers.setup;

public static class TerminateCommandPrinter
{
    public static Embed CreateTerminateCommand()
    {
        var embed = new EmbedBuilder()
            .WithTitle("Configs have been stopped")
            .WithColor(Color.Red)
            .Build();
        return embed;
    }
}