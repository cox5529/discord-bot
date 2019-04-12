using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord_bot.Services;

namespace Discord_bot.Modules {
    public class PublicModule : ModuleBase<SocketCommandContext> {
        public PictureService _pictureService { get; set; }
        public ConfigurationService _configurationService { get; set; }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("cat")]
        public async Task CatAsync() {
            var stream = await _pictureService.GetCatPictureAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        [Command("humpday")]
        public async Task HumpDayAsync() {
            var stream = await _pictureService.GetLocalImage("dave_camel");
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "hump_day.png");
        }

        [Command("bigqingus")]
        public async Task BigQingusAsync() {
            var stream = await _pictureService.GetLocalImage("big_qingus");
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "big_qingus.png");
        }

        [Command("semester")]
        public async Task SetSemesterTaskAsync(int? semester) {
            var channelConfig = await _configurationService.ReadChannelAsync(Context.Channel.Id);
            if (semester != null) {
                channelConfig.Semester = semester.Value;
                await _configurationService.WriteChannelAsync(channelConfig);
                await Context.Channel.SendMessageAsync("Semester has been set to " + semester);
            } else {
                await Context.Channel.SendMessageAsync("Current semester is set to " + channelConfig.Semester);
            }
        }

        [Command("enroll")]
        public async Task GetEnrollmentAsync() {
            var channelConfig = await _configurationService.ReadChannelAsync(Context.Channel.Id);

        }

        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null) {
            user = user ?? Context.User;
            await ReplyAsync(user.ToString());
        }

        [Command("echo")]
        public Task EchoAsync([Remainder] string text)
            => ReplyAsync('\u200B' + text);

        [Command("list")]
        public Task ListAsync(params string[] objects)
            => ReplyAsync("You listed: " + string.Join("; ", objects));
    }
}


/*
TODO:

1. Query enrollment in a specific section
2. Spam bot

 */
