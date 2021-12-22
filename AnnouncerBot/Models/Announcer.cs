using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Threading.Tasks;

namespace AnnouncerBot.Models
{
    public static class Announcer
    {
        private static DiscordChannel ChannelToJoin { get; set; } = null;
        static VoiceNextConnection Connection { get; set; } = null;
        static VoiceNextExtension Vnext { get; set; } = null;
        static Dictionary<DiscordUser, string> AnnouncableNames { get; set; } = null;
        public static async Task GetSpeachSynthisizerFiles(CommandContext context)
        {
            var members = context.Guild.Members.Values;
            using SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            AnnouncableNames = new Dictionary<DiscordUser, string>();
            foreach (var member in members)
            {
                synthesizer.SetOutputToWaveFile($@"C:\Users\Domjr\Desktop\Projects\AnnouncerBotDiscord\AnnouncerBot\Assets\{member.Username}.wav", new SpeechAudioFormatInfo(48000, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));
                PromptBuilder builder = new();
                builder.AppendText($"{member.DisplayName} has joined the chat");
                synthesizer.Speak(builder);
                AnnouncableNames.Add(member, $@"C:\Users\Domjr\Desktop\Projects\AnnouncerBotDiscord\AnnouncerBot\Assets\{member.Username}.wav");
            }
        }
        internal static Task AnnounceUserJoined(CommandContext context)
        {
            context.Client.VoiceStateUpdated += AnnounceName_VoiceStateUpdated;
            return Task.CompletedTask;
        }
        internal static Task StopAnnouncing(CommandContext context)
        {
            context.Client.VoiceStateUpdated -= AnnounceName_VoiceStateUpdated;
            return Task.CompletedTask;
        }
        private static Task AnnounceName_VoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            if (e.Channel == null || (e.Before.IsSelfVideo || e.Before.IsSelfDeafened || e.Before.IsSelfMuted || e.Before.IsSelfStream || e.Before.IsSuppressed || e.Before.IsServerMuted || e.Before.IsServerDeafened)
                                  || (e.After.IsSelfVideo || e.After.IsSelfDeafened || e.After.IsSelfMuted || e.After.IsSelfStream || e.After.IsSuppressed || e.After.IsServerMuted || e.After.IsServerDeafened))
            {
                return Task.CompletedTask;
            }
            var joinChat = Task.Run(() =>
            {
                if (!e.Channel.Users.Contains(sender.CurrentUser))
                {

                    ChannelToJoin ??= e.Channel;
                    ChannelToJoin.ConnectAsync();
                    ChannelToJoin = null;
                }
                
                    if(!e.User.IsBot)
                    {
                        var talkInChat = Task.Run(async () =>
                        {
                            Vnext = sender.GetVoiceNext();
                            Connection = Vnext.GetConnection(e.Guild);

                            while (Connection == null)
                            {
                                Connection = Vnext.GetConnection(e.Guild);
                            }
                            await Talk(Connection, e.User);

                            Connection.Disconnect();
                        });
                    }
                    
                
            });

            return Task.WhenAll(joinChat);
        }
        public static async Task Talk(VoiceNextConnection connection, DiscordUser UserToAnnounce)
        {

            var filePath = $"{AnnouncableNames[UserToAnnounce]}";
            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{filePath}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
            Stream pcm = ffmpeg.StandardOutput.BaseStream;
            VoiceTransmitSink transmit = connection.GetTransmitSink();
            Console.WriteLine("Test 1 passed");
            await pcm.CopyToAsync(transmit);
            transmit = null;
        }
    }
}
