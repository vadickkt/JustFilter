using Discord;
using JustFilter.infrastructure.datastore.mongo.config;

namespace JustFilter.presentation.printers.setup;

public static class SetupPrinter
{
    public static MessageComponent BuildHelpButtons()
    {
        var builder = new ComponentBuilder();
            
        var buttons = BuildButtons();
        foreach (var button in buttons)
        {
            builder.WithButton(button);
        }

        return builder.Build();
    }
    
    public static Embed BuildHelpMessage()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("You haven't created a config yet, you can do it via commands or buttons below")
            .WithColor(Color.Blue);
        
        return embedBuilder.Build();
    }

    public static Embed BuildListOfAvailableConfigs(List<ConfigData> configs)
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Select a config to configure JustFilter in a channel or just manage your configs ")
            .WithColor(Color.Blue);

        for (var i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
            embedBuilder.AddField(
                $"#{i + 1} • 🧾 `{config.Name}`",
                $"📄 **Description:** {config.Description}\n🔑 **ID:** `{config.Id}`",
                inline: false
            );
        }

        return embedBuilder.Build();
    }

    public static MessageComponent BuildSetupComponents(List<ConfigData> configs)
    {
        Console.WriteLine($"[BuildSetupComponents] Start - Config count: {configs?.Count ?? 0}");

        if (configs == null || configs.Count == 0)
        {
            Console.WriteLine("[BuildSetupComponents] WARNING: Empty or null config list!");
            throw new ArgumentException("Cannot build setup components with empty config list.");
        }

        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select a config")
            .WithCustomId("setup_menu")
            .WithMinValues(1) // changed from 0 to 1 to avoid invalid config state
            .WithMaxValues(configs.Count);

        for (var i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
            Console.WriteLine($"[BuildSetupComponents] Adding config option: Name={config.Name}, Id={config.Id}");
            menuBuilder.AddOption($"#{i + 1} {config.Name}", config.Id.ToString());
        }

        var builder = new ComponentBuilder()
            .WithSelectMenu(menuBuilder);

        var buttons = BuildButtons();
        Console.WriteLine($"[BuildSetupComponents] Adding {buttons.Count} buttons.");

        foreach (var button in buttons)
        {
            Console.WriteLine($"[BuildSetupComponents] Button: Label={button.Label}, CustomId={button.CustomId}");
            builder.WithButton(button);
        }

        Console.WriteLine("[BuildSetupComponents] Finished building components.");
        return builder.Build();
    }


    private static List<ButtonBuilder> BuildButtons()
    {
        var addNewConfigButton = new ButtonBuilder()
            .WithCustomId("create_config_button")
            .WithLabel("Create Config")
            .WithStyle(ButtonStyle.Success);
        
        var deleteConfigButton = new ButtonBuilder()
            .WithCustomId("delete_config_button")
            .WithLabel("Delete Config")
            .WithStyle(ButtonStyle.Danger);
        
        var editConfigButton = new ButtonBuilder()
            .WithCustomId("edit_config_button")
            .WithLabel("Edit Config")
            .WithStyle(ButtonStyle.Primary);
        
        return [addNewConfigButton, deleteConfigButton, editConfigButton];
    }
}