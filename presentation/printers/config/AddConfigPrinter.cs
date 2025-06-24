using Discord;

namespace JustFilter.presentation.printers.config;

public static class AddConfigPrinter
{
    public static Embed PrintConfigWasCreated(string configName)
    {
        var embed = new EmbedBuilder()
            .WithTitle($"Config {configName} has been saved.")
            .WithColor(Color.Green);
        
        return embed.Build();
    }
}