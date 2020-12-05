using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DiscordBotTemplateV2.Data
{
    public class BotContextFactory : IDesignTimeDbContextFactory<BotContext>
    {
        public BotContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json").Build();

            var optionsBuilder = new DbContextOptionsBuilder<BotContext>().UseSqlite(config.GetValue<string>("DbConnectionString"));
            return new BotContext(optionsBuilder.Options);
        }
    }
}