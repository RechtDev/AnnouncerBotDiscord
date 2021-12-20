using AnnouncerBot.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnouncerBot.Commands
{
    class ConfigModule:BaseCommandModule
    {
        [Command("Start-NA")]
        public async Task StartCommand(CommandContext ctx, DiscordChannel channel = null)
        {
            ChannelWatcher OverSeer = new(ctx.Guild, ctx.Client.GetVoiceNext());
            await OverSeer.MonitorChannelAudience();
            OverSeer.ActiveUsersChanged += OverSeer_ActiveUsersChanged;
           
        }

        private void OverSeer_ActiveUsersChanged(object sender, List<DiscordUser> e)
        {
            Console.WriteLine("Testing...");
        }
    }
}
