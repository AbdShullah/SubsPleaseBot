using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBotTemplateV2.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandHandlingService _commandHandler;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;
        private readonly ILogger<StartupService> _logger;

        public StartupService(
            DiscordSocketClient discord,
            CommandHandlingService commandHandler,
            IServiceProvider services,
            IConfiguration config,
            ILogger<StartupService> logger)
        {
            _discord = discord;
            _commandHandler = commandHandler;
            _services = services;
            _config = config;
            _logger = logger;
        }

        public async Task StartAsync()
        {
            _services.GetRequiredService<LoggingService>();

            string token = _config.GetValue<string>("DiscordToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception("Please enter your bot's token into the `config.json` file found in the applications root directory.");
            }
            await _discord.LoginAsync(TokenType.Bot, token);
            _config["DiscordToken"] = string.Empty;
            await _discord.StartAsync();
            await _discord.SetGameAsync($"{_config.GetValue<string>("Prefix")}help", type: ActivityType.Listening);

            await _commandHandler.InitializeAsync();
        }
    }
}