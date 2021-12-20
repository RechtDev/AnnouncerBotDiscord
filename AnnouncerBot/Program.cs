using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.VoiceNext;
using DSharpPlus.CommandsNext;
using AnnouncerBot.Commands;

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
                Intents = DiscordIntents.All
            });
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] {"!ab "}
            });

            commands.RegisterCommands<ConfigModule>();
            discord.UseVoiceNext();
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
