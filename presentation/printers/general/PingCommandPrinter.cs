using Discord;

namespace JustFilter.presentation.printers.general;

public static class PingCommandPrinter
{
    public static Embed CreateRespondWithLatency(int latency)
    {
        var embed = new EmbedBuilder()
            .WithTitle($"🏓 Pong! Latency: {latency}ms")
            .WithColor(Color.Blue)
            .Build();
        return embed;
    }
}