using DSharpPlus.Entities;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;

namespace AnnouncerBot.Models
{
    class ChannelWatcher
    {
        private List<DiscordUser> _activeUsers = new List<DiscordUser>();

        public List<DiscordUser> ActiveUsers
        {
            get { return _activeUsers; }
            set
            {
                _activeUsers = value;
                OnActiveUsersChanged();
            }
        }
        public DiscordShardedClient ShardedClient { get; set; }
        public DiscordGuild Guild { get; set; }
        VoiceNextExtension Vnext { get; set; }
        DiscordUser User { get; set; }
        public VoiceNextConnection Connection { get; set; }
        public IEnumerable<KeyValuePair<ulong, DiscordChannel>> ServerChannels { get; set; }
        public event EventHandler<List<DiscordUser>> ActiveUsersChanged;
        public ChannelWatcher(DiscordGuild guild, VoiceNextExtension voiceNext)
        {
            this.Guild = guild;
            this.Vnext = voiceNext;
            ServerChannels = this.Guild.Channels.Where(x => x.Value.Type == ChannelType.Voice);
        }
        public Task MonitorChannelAudience()
        {
            foreach (var channel in ServerChannels)
            {
                ActiveUsers.AddRange(channel.Value.Users.ToList());
            }
            return Task.CompletedTask;
        }
        protected virtual void OnActiveUsersChanged()
        {
            if (ActiveUsersChanged != null)
            {
                ActiveUsersChanged(this, ActiveUsers);
            }
        }        
    }
}
