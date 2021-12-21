using AnnouncerBot.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnouncerBot.Commands
{
    class ConfigModule : BaseCommandModule
    {

        [Command("Start-NA")]
        public async Task StartNACommand(CommandContext ctx)
        {
            await Announcer.GetSpeachSynthisizerFiles(ctx);
            await Announcer.AnnounceUserJoined(ctx);
        }
        [Command("Stop-NA")]
        public async Task EndNACommand(CommandContext ctx)
        {
            await Announcer.StopAnnouncing(ctx);
        }
    }
}
