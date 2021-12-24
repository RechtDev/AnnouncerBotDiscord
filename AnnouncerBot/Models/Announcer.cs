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
    public class Announcer
    {
        private static bool firstTime = true;
        private static bool created = false;
        private  DiscordChannel ChannelToJoin { get; set; } = null;
        private  CommandContext Context { get; set; } = null;

        internal async static Task<bool> NotifyStatus(CommandContext ctx)
        {
            if (!created)
            {
                DiscordEmbedBuilder nowActiveEmbed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Orange,
                    Title = "Now Announicng",
                    Description = "Discord nicknames will now be announced in VCs. To change names spoken, 1st " +
                    "change Discord nickname then restart name announcments",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = $"Success- Executed By: {ctx.Member.Username}" }
                };
                await ctx.RespondAsync(nowActiveEmbed);
                created = true;
                return created;
            }
            else
            {
                DiscordEmbedBuilder alreadyActiveEmbed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Orange,
                    Title = "Already Announcing Names",
                    Description = "This command is already runnning, did you mean to end? Use '!ab stop-na'",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Oppsies" }
                };
                await ctx.RespondAsync(alreadyActiveEmbed);
                return created;
            }
            
        }

        private VoiceNextConnection Connection { get; set; } = null;
        private Queue<VoiceStateUpdateEventArgs> UsersWaitingToBeAnnounced { get; set; } 
        private VoiceNextExtension Vnext { get; set; } = null;
        private Dictionary<DiscordUser, string> AnnouncableNames { get; set; } = null;
        
        public Announcer(CommandContext context)
        {
            Context = context;
            AnnouncableNames = new Dictionary<DiscordUser, string>();
            UsersWaitingToBeAnnounced = new();
            AssignHandlers();
            GetSpeachSynthisizerFiles();
        }

        private void AssignHandlers()
        {
            Context.Client.VoiceStateUpdated += AnnounceName_VoiceStateUpdated;
        }
        public void UnsubscribeHandler()
        {
            Context.Client.VoiceStateUpdated -= AnnounceName_VoiceStateUpdated;
        }
        private void GetSpeachSynthisizerFiles()
        {
            var members = Context.Guild.Members.Values;
            using SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            foreach (var member in members)
            {
                synthesizer.SetOutputToWaveFile($@"C:\Users\Domjr\Desktop\Projects\AnnouncerBotDiscord\AnnouncerBot\Assets\{member.Username}.wav", new SpeechAudioFormatInfo(48000, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));
                PromptBuilder builder = new();
                builder.AppendText($"{member.DisplayName} has joined the chat");
                synthesizer.Speak(builder);
                AnnouncableNames.Add(member, $@"C:\Users\Domjr\Desktop\Projects\AnnouncerBotDiscord\AnnouncerBot\Assets\{member.Username}.wav");
            }
        }
        private Task AnnounceName_VoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            UsersWaitingToBeAnnounced.Enqueue(e);
            if (Context.Guild == e.Guild)
            {
                
                Console.WriteLine(UsersWaitingToBeAnnounced.Count + " Inside user join");
                if (!firstTime)
                {
                    if (e.Channel == null || (e.After.IsSelfVideo || e.After.IsSelfDeafened || e.After.IsSelfMuted || e.After.IsSelfStream || e.After.IsSuppressed || e.After.IsServerMuted || e.After.IsServerDeafened)
                                      || (e.Before.IsSelfVideo || e.Before.IsSelfDeafened || e.Before.IsSelfMuted || e.Before.IsSelfStream || e.Before.IsSuppressed || e.Before.IsServerMuted || e.Before.IsServerDeafened))
                    {
                        UsersWaitingToBeAnnounced.Dequeue();
                        Console.WriteLine(UsersWaitingToBeAnnounced.Count + " Inside User Disconnect");
                        return Task.CompletedTask;
                    }
                }
                while (UsersWaitingToBeAnnounced.Count >= 3)
                {
                    Task.WaitAll(Task.CompletedTask);
                }
                
                var joinChat = Task.Run(() =>
                {
                    if (!e.Channel.Users.Contains(sender.CurrentUser))
                    {

                        ChannelToJoin ??= e.Channel;
                        ChannelToJoin.ConnectAsync();
                        ChannelToJoin = null;
                    }
                    if (!e.User.IsBot)
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
                            UsersWaitingToBeAnnounced.Dequeue();
                            UsersWaitingToBeAnnounced.Dequeue();
                            Console.WriteLine(UsersWaitingToBeAnnounced.Count + " Inside Bot Disconnect");
                            firstTime = false;
                        });
                    }
                });
                
                return Task.WhenAll(joinChat);
            }
            UsersWaitingToBeAnnounced.Dequeue();
            Console.WriteLine(UsersWaitingToBeAnnounced.Count + " Inside Wrong guild");
            return Task.CompletedTask;  
        }
        private async Task Talk(VoiceNextConnection connection, DiscordUser UserToAnnounce)
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
            await pcm.CopyToAsync(transmit);
            await pcm.DisposeAsync();
        }
    }
}
