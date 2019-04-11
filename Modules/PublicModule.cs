using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord_bot.Services;

namespace Discord_bot.Modules {
    public class PublicModule : ModuleBase<SocketCommandContext> {
        public PictureService PictureService { get; set; }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("cat")]
        public async Task CatAsync() {
            var stream = await PictureService.GetCatPictureAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        [Command("bigqingus")]
        public async Task BigQingusAsync() {
            var stream = await PictureService.GetBigQingus();
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "big_qingus.png");
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