using Discord;
using Discord.Commands;

namespace SubsPleaseBot.Embeds
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