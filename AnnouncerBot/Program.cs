using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Threading.Tasks;
using DSharpPlus;

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
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = secretToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

    }
}
