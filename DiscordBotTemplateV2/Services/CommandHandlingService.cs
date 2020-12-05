using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotTemplateV2.Embeds;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DiscordBotTemplateV2.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        private readonly ILogger<CommandHandlingService> _logger;

        public CommandHandlingService(
            DiscordSocketClient discord,
            CommandService commands,
            IConfiguration config,
            IServiceProvider services,
            ILogger<CommandHandlingService> logger)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _services = services;
            _logger = logger;

            _discord.MessageReceived += MessageReceivedAsync;
            _commands.CommandExecuted += CommandExecutedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (rawMessage is not SocketUserMessage message)
            {
                return;
            }
            if (message.Source != MessageSource.User)
            {
                return;
            }
            await ProcessCommandAsync(message);
        }

        private async Task ProcessCommandAsync(SocketUserMessage message)
        {
            int argPos = 0;
            if (!(message.HasStringPrefix(_config.GetValue<string>("Prefix"), ref argPos) || message.HasMentionPrefix(_discord.CurrentUser, ref argPos)))
            {
                return;
            }
            var context = new SocketCommandContext(_discord, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
            {
                return;
            }
            if (result.IsSuccess)
            {
                return;
            }
            _logger.LogTrace("Command failed: {command} | {error} | {errorReason}", command.Value?.Name, result.Error, result.ErrorReason);
            await context.Channel.SendMessageAsync(embed: new ErrorEmbed(result).Build());
        }
    }
}