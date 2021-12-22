using AnnouncerBot.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
