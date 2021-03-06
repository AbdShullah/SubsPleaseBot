﻿using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SubsPleaseBot.Embeds;
using SubsPleaseBot.Results;

namespace SubsPleaseBot.Modules
{
    [Name("Informations")]
    [Summary("Useful informations about bot")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly ILogger<InfoModule> _logger;

        public InfoModule(CommandService commands, IConfiguration config, ILogger<InfoModule> logger)
        {
            _commands = commands;
            _config = config;
            _logger = logger;
        }

        [Command("help")]
        [Summary("Shows all commands")]
        public async Task<RuntimeResult> HelpCmd()
        {
            try
            {
                foreach (var module in _commands.Modules.OrderBy(x => x.Name))
                {
                    await Context.User.SendMessageAsync(embed: new CommandsEmbed(module, _config.GetValue<string>("Prefix")).Build());
                }
                await ReplyAsync("Please check your DM for the list of commands.");
            }
            catch (HttpException e)
            {
                switch (e.DiscordCode)
                {
                    case 50007:
                        return new DirectMessageError();

                    default:
                        throw;
                }
            }
            return new SuccessResult();
        }

        [Command("help")]
        [Summary("Shows all commands")]
        public async Task HelpCmd(string command)
        {
            var search = _commands.Search(command);
            if (!search.IsSuccess)
            {
                return;
            }
            foreach (var cmd in search.Commands)
            {
                await ReplyAsync(embed: new CommandEmbed(cmd.Command, _config.GetValue<string>("Prefix")).Build());
            }
        }

        [Command("info")]
        [Alias("about")]
        [Summary("Information about bot")]
        public async Task InfoCmd()
        {
            var app = await Context.Client.GetApplicationInfoAsync();
            await ReplyAsync(embed: new InfoEmbed(app).Build());
        }
    }
}