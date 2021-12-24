using AnnouncerBot.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace AnnouncerBot.Commands
{
    class ConfigModule : BaseCommandModule
    {
        private DependecyFactory anon { get; set; }
        [Command("Start-NA")]
        public async Task StartNACommand(CommandContext ctx)
        {
            anon = new DependecyFactory(ctx);
        }
        [Command("Stop-NA")]
        public async Task EndNACommand(CommandContext ctx)
        {
            anon.announcer.UnsubscribeHandler();
            anon = null;
        }
    }
}
