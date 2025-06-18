using Discord; 
using JustFilter.infrastructure.database.mongo.entities;

namespace JustFilter.presentation.printers;

public static class ConfigPrinter
{
    public static Embed PrintConfigs(List<ConfigData> configs)
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("🛠️ Configs: ")
            .WithColor(Color.Blue);

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

    public static Embed PrintUpdateMessage()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Update Config")
            .WithColor(Color.Gold)
            .WithDescription("Select the config you want to update");

        return embedBuilder.Build();
    }
}
