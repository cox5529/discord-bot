using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord_bot.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Discord_bot.Services {
    public class ConfigurationService {

        private readonly IConfigurationRoot _configuration;

        public ConfigurationService(IConfigurationRoot configuration) {
            _configuration = configuration;
        }

        public async Task<ChannelConfiguration> ReadChannelAsync(ulong id) {
            var config = await ReadAsync();
            var server = config.Servers.FirstOrDefault(x => x.DiscordId == id);
            if (server == null) {
                server = new ChannelConfiguration() {
                    Semester = 1199
                };
            }
            return server;
        }

        public async Task WriteChannelAsync(ChannelConfiguration serverConfig) {
            var config = await ReadAsync();
            var server = config.Servers.FirstOrDefault(x => x.DiscordId == serverConfig.DiscordId);
            if (server != null) {
                config.Servers.Remove(server);
            }
            config.Servers.Add(server);
            await WriteAsync(config);
        }

        public async Task<Configuration> ReadAsync() {
            string file = "";
            try {
                await File.ReadAllTextAsync(_configuration["settings_file"]);
                var result = JsonConvert.DeserializeObject<Configuration>(file);
                return result;
            } catch {
                return new Configuration() {
                    Servers = new List<ChannelConfiguration>()
                };
            }
        }

        public async Task WriteAsync(Configuration config) {
            string file = JsonConvert.SerializeObject(config);
            await File.WriteAllTextAsync(_configuration["settings_file"], file);
        }
    }
}