using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_bot.Models;
using Discord_bot.SelectTable.Models;
using Discord_bot.SelectTable.Sql.MiniDatabase;
using Discord_bot.Services;

namespace Discord_bot.Modules {
    public class PublicModule : ModuleBase<SocketCommandContext> {
        public PictureService PictureService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public EnrollmentService EnrollmentService { get; set; }
        public CourseDatabaseService CourseDatabaseService { get; set; }

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

        [Command("humpday")]
        public async Task HumpDayAsync() {
            if (DateTime.Now.DayOfWeek == System.DayOfWeek.Wednesday) {
                var stream = await PictureService.GetLocalImage("dave_camel");
                stream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(stream, "hump_day.png");
            } else {
                await Context.Channel.SendMessageAsync("It isn't Wednesday, you bitch.");
            }
        }

        [Command("dave")]
        [Alias("davedrews")]
        public async Task DaveAsync() {
            var stream = await PictureService.GetLocalImage("dave");
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "dave.png");
        }

        [Command("bigqingus")]
        public async Task BigQingusAsync() {
            var stream = await PictureService.GetLocalImage("big_qingus");
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "big_qingus.png");
        }

        [Command("kaqingus")]
        [Alias("kaqinghua")]
        public async Task KaQingusAsync() {
            var stream = await PictureService.GetLocalImage("kaqingus");
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "kaqingus.png");
        }

        [Command("semester")]
        public async Task SetSemesterTaskAsync(int? semester) {
            var channelConfig = await ConfigurationService.ReadChannelAsync(Context.Channel.Id);
            if (semester != null) {
                channelConfig.Semester = semester.Value;
                await ConfigurationService.WriteChannelAsync(channelConfig);
                await Context.Channel.SendMessageAsync("Semester has been set to " + semester);
            } else {
                await Context.Channel.SendMessageAsync("Current semester is set to " + channelConfig.Semester);
            }
        }

        [Command("course")]
        public async Task GetEnrollmentAsync(params string[] options) {
            var channelConfig = await ConfigurationService.ReadChannelAsync(Context.Channel.Id);
            List<CourseModel> courses;
            switch (options.Length) {
                case 0:
                    await Context.Channel.SendMessageAsync(
                        "No results were fetched because no parameters were specified.");
                    return;
                case 1:
                    courses = await EnrollmentService.GetCoursesById(channelConfig.Semester, options[0], "");
                    break;
                case 2:
                    courses = await EnrollmentService.GetCoursesById(channelConfig.Semester, options[0], options[1]);
                    break;
                default:
                    return;
            }

            var table = new PrettyTable<CourseModel> { Data = courses };
            table.AddColumn("Catalog", x => x.Department + " " + x.CatalogNumber);
            table.AddColumn("Section", x => x.SectionNumber);
            table.AddColumn("Title", x => x.Name);
            table.AddColumn("Instructor", x => x.Instructor);
            table.AddColumn("Enrollment", x => x.Enrolled + "/" + x.Size);

            await SplitAndSendMessageAsync(Context.Channel, "```" + table.BuildTable() + "```");
        }

        [Command("select")]
        public async Task QueryCoursesAsync([Remainder] string query) {
            query = "select " + query;

            var channelConfig = await ConfigurationService.ReadChannelAsync(Context.Channel.Id);
            CourseDatabaseService.Semester = channelConfig.Semester;

            var data = await CourseDatabaseService.Query(query);

            var table = new PrettyTable<string[]> { Data = data.ToList() };
            for (var i = 0; i < data[0].Length; i++) {
                var i1 = i;
                table.AddColumn(data[0][i], x => x[i1]);
            }

            await SplitAndSendMessageAsync(Context.Channel, "```" + table.BuildTable() + "```");
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

        private async Task SplitAndSendMessageAsync(ISocketMessageChannel channel, string message) {
            var messages = new List<string>();
            var limit = 2000;
            var prefix = "";
            var postfix = "";
            if (message.StartsWith("```")) {
                limit = 1994;
                prefix = "```";
                postfix = "```";
            }

            while (message.Length > limit) {
                var index = limit;
                if (message.Substring(0, limit).Contains("\n")) {
                    index = message.Substring(0, limit).LastIndexOf("\n", StringComparison.Ordinal);
                }

                messages.Add(message.Substring(0, index));
                message = message.Substring(index);
            }

            if (message.Length > 0) {
                messages.Add(message);
            }

            foreach (var s in messages) {
                var toSend = s;
                if (!toSend.StartsWith(prefix)) toSend = prefix + s;
                if (!toSend.EndsWith(postfix)) toSend += postfix;
                await channel.SendMessageAsync(toSend);
            }
        }
    }
}


/*
TODO:

1. SQL style enrollment queries
2. Spam bot

 */
