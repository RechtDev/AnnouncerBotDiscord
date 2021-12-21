using AnnouncerBot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using System;
using System.Threading.Tasks;

namespace AnnouncerBot.Models
{
    public class AnnouncerBot
    {
        private DiscordClient _discordClient;
        public AnnouncerBot(string token)
        {
            _discordClient = InitBot(token);
            AddCommandModules();
            _discordClient.UseVoiceNext();
        }

        public async Task Connect()
        {
            await _discordClient.ConnectAsync();
            await Task.Delay(-1);
        }

        private static DiscordClient InitBot(string token)
        {
            return new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All

            });

        }
        private void AddCommandModules()
        {
            var commands = _discordClient.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!ab " }
            });
            commands.RegisterCommands<ConfigModule>();
        }

        private static Task Discord_VoiceStateUpdated(DiscordClient sender, DSharpPlus.EventArgs.VoiceStateUpdateEventArgs e)
        {
            Console.WriteLine(e.User.Username);
            return Task.CompletedTask;
        }
    }
}
