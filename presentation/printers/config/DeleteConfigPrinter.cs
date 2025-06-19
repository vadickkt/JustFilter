using Discord;
using JustFilter.data.utils;
using JustFilter.infrastructure.database.mongo.config;

namespace JustFilter.presentation.printers;

public static class DeleteConfigPrinter
{
    public static Embed PrintDeleteMessage()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Delete Config")
            .WithColor(Color.Red)
            .WithDescription("Select the config you want to delete");

        return embedBuilder.Build();
    }

    public static MessageComponent BuildConfigDeleteComponents(List<ConfigData> configs)
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select a config")
            .WithCustomId("delete_config_menu")
            .WithMinValues(1)
            .WithMaxValues(configs.Count);

        foreach (var config in configs)
        {
            menuBuilder.AddOption(config.Name, config.Id.ToString(), config.Description.Truncate(50));
        }

        var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);
        return builder.Build();
    }
}