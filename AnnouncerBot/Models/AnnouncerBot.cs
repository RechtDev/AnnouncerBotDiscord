using AnnouncerBot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AnnouncerBot.Models
{
    public class AnnouncerBot
    {
        private DiscordShardedClient _discordClient;
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
        private static DiscordShardedClient InitBot(string token)
        {
            return new DiscordShardedClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All

            });
        }
        private void AddCommandModules()
        {
            var commands = _discordClient.UseCommandsNextAsync(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!ab " },
                Services = AddServices()
            });
            commands.RegisterCommands<ConfigModule>();
        }
        private ServiceProvider AddServices()
        {
            var services = new ServiceCollection()
               .AddSingleton<DependecyFactory>()
               .BuildServiceProvider();
            return services;
        }
    }
}
