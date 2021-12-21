using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.VoiceNext;
using DSharpPlus.CommandsNext;
using AnnouncerBot.Commands;
using AnnouncerBot.Models;

namespace AnnouncerBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            string secretToken = ConfigurationManager.AppSettings.Get("BotToken");
            var bot = new Models.AnnouncerBot(secretToken);
            await bot.Connect();    
        }

    }
}
