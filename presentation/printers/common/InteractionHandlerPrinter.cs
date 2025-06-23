using Discord;
using MongoDB.Bson;

namespace JustFilter.presentation.printers.common;

public static class InteractionHandlerPrinter
{
    public static Embed CreateMessageAboutDeletedMessage()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Message was deleted")
            .WithColor(Color.Red);

        return embedBuilder.Build();
    }

    public static MessageComponent BuildDeletedMessageInfoButton(ObjectId deletedMessageId, string baseUrl)
    {
        var url = $"${baseUrl}/{deletedMessageId}";

        var infoButton = new ButtonBuilder()
            .WithLabel("View Information")
            .WithUrl(url)
            .WithStyle(ButtonStyle.Link);

        var builder = new ComponentBuilder()
            .WithButton(infoButton);

        return builder.Build();
    }
}