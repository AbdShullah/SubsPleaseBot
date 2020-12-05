using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SubsPleaseBot.Data;
using SubsPleaseBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace SubsPleaseBot
{
    internal class Startup
    {
        public Startup(string[] args)
        {
        }

        public async Task StartAsync()
        {
            using var services = ConfigureServices();
            await services.GetRequiredService<StartupService>().StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        public IConfiguration LoadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
            return builder.Build();
        }

        private ServiceProvider ConfigureServices()
        {
            var config = LoadConfig();
            LoggingService.ConfigureLogger(config);

            return new ServiceCollection()
                // Singletons
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig { LogLevel = config.GetValue<LogSeverity>("DiscordLogLevel") }))
                .AddSingleton<HttpClient>()
                // Services
                .AddSingleton<StartupService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<LoggingService>()
                .AddDbContext<BotContext>(options => options.UseSqlite(config.GetValue<string>("DbConnectionString")))
                // Transients
                // Config
                .AddLogging(configure => configure.AddSerilog())
                .AddSingleton(config)
                .BuildServiceProvider();
        }
    }
}