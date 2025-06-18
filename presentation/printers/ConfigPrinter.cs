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
                $"📄 **Description:** {config.Description}\n🔑 **ID:** `{config.Id}`",
                inline: false
            );
        }

        return embedBuilder.Build();
    }
}
