using Discord;

namespace JustFilter.presentation.printers.common;

public static class BaseCommandPrinter
{
    public static Embed CreateMessageNoPermission()
    {
        var embed = new EmbedBuilder()
            .WithTitle("Failure")
            .WithColor(Color.Blue)
            .WithDescription(
                "You do not have permission to execute this command\n" +
                "You need **JustFilterAdmin** rule to run this bot"
            )
            .Build();
        
        return embed;
    }
}