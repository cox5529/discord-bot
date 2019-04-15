using System.Collections.Generic;

namespace Discord_bot.Models {
    public class Configuration {
        public List<ChannelConfiguration> Servers { get; set; }
    }

    public class ChannelConfiguration {
        public ulong DiscordId { get; set; }
        public int Semester { get; set; }
    }
}