using Discord;
using Discord.Rest;

namespace DiscordBotTemplateV2.Embeds
{
    public class InfoEmbed : EmbedBuilder
    {
        public InfoEmbed(RestApplication app)
        {
            Title = $"About {app.Name}";
            Description = app.Description;
            ThumbnailUrl = app.IconUrl;
            AddField("Author:", app.Owner);
            Color = EmbedColors.Info;
        }
    }
}