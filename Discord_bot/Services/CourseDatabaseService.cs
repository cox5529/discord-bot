using System.Linq;
using System.Threading.Tasks;
using Discord_bot.Models;
using Discord_bot.SelectTable.Sql.MiniDatabase;
using Microsoft.Extensions.Configuration;

namespace Discord_bot.Services {
    public class CourseDatabaseService : MiniDatabase<CourseModel> {
        private readonly EnrollmentService _enrollmentService;

        public int Semester { get; set; }

        public CourseDatabaseService(EnrollmentService enrollmentService) {
            _enrollmentService = enrollmentService;
        }

        public override async Task<Table<CourseModel>> FetchMissingTable(string identifier) {
            var list = await _enrollmentService.GetCoursesById(Semester, identifier, "");
            var table = new Table<CourseModel> {
                Data = list,
                Identifier = identifier
            };

            return table;
        }
    }
}