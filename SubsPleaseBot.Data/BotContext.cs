using Microsoft.EntityFrameworkCore;

namespace SubsPleaseBot.Data
{
    public class BotContext : DbContext
    {
        public BotContext(DbContextOptions options) : base(options)
        {
        }
    }
}