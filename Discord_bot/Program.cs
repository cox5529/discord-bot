using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord_bot.Services;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Discord_bot {
    public class Program {

        private readonly IConfigurationRoot _configuration;

        public Program() {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public async Task MainAsync() {
            using (var services = ConfigureServices()) {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await client.LoginAsync(TokenType.Bot, _configuration["token"]);
                await client.StartAsync();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage log) {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices() {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .AddSingleton<ConfigurationService>()
                .AddSingleton<EnrollmentService>()
                .AddSingleton<CourseDatabaseService>()
                .AddSingleton(_configuration)
                .BuildServiceProvider();
        }

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();
    }
}