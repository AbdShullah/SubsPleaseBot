using Microsoft.EntityFrameworkCore;

namespace DiscordBotTemplateV2.Data
{
    public class BotContext : DbContext
    {
        public BotContext(DbContextOptions options) : base(options)
        {
        }
    }
}