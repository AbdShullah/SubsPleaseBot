using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace SubsPleaseBot.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(DiscordSocketClient discord, CommandService commands, ILogger<LoggingService> logger)
        {
            _discord = discord;
            _commands = commands;
            _logger = logger;
            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        public static void ConfigureLogger(IConfiguration config)
        {
            string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Logger(l => l
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code, restrictedToMinimumLevel: config.GetValue<LogEventLevel>("ConsoleLogLevel"))
                    .WriteTo.File("logs/log.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: config.GetValue<LogEventLevel>("FileLogLevel"), outputTemplate: outputTemplate))
                 .Enrich.FromLogContext()
                 .MinimumLevel.Is(LogEventLevel.Verbose)
                 .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .CreateLogger();
        }

        private Task OnLogAsync(LogMessage msg)
        {
            string logText = $"[Discord] {msg.Exception?.ToString() ?? msg.Message}";
            var severity = msg.Severity switch // Translate discord net logging levels
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Debug,
                LogSeverity.Debug => LogLevel.Trace, // Switching debug with verbose because discord.net is stupid
                _ => throw new System.NotImplementedException(),
            };
            _logger.Log(severity, logText);
            return Task.CompletedTask;
        }
    }
}