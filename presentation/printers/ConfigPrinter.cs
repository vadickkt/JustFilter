using Discord; 
using JustFilter.infrastructure.database.mongo.entities;

namespace JustFilter.presentation.printers;

public static class ConfigPrinter
{
    public static Embed BuildEmbed(List<ConfigData> configs)
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("🛠️ Configs: ")
            .WithColor(Color.Blue)
            .WithFooter(footer => footer.Text = "JustFilter • Config Viewer")
            .WithCurrentTimestamp();

        var index = 1;
        foreach (var config in configs)
        {
            embedBuilder.AddField(
                $"#{index++} • 🧾 `{config.Name}`",
                $"📄 **Desciption:** {Truncate(config.Description, 200)}\n🔑 **ID:** `{config.Id}`",
                inline: false
            );
        }

        return embedBuilder.Build();
    }

    private static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(text)) return "—";
        return text.Length <= maxLength ? text : text.Substring(0, maxLength - 3) + "...";
    }
}
