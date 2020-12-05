using Discord;
using Discord.Commands;

namespace DiscordBotTemplateV2.Embeds
{
    public class ErrorEmbed : EmbedBuilder
    {
        public ErrorEmbed(IResult result)
        {
            Title = "Error";
            Description = result.ErrorReason;
            Color = EmbedColors.Error;
        }
    }
}