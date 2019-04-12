using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Discord_bot.Models;
using Microsoft.Extensions.Configuration;

namespace Discord_bot.Services {


    public class EnrollmentService {

        private static HttpClient _client = new HttpClient();
        private readonly ConfigurationService _configurationService;
        private readonly IConfigurationRoot _configuration;

        public EnrollmentService(ConfigurationService configurationService, IConfigurationRoot configuration) {
            _configurationService = configurationService;
            _configuration = configuration;
        }

        public async Task<List<CourseModel>> GetCoursesById(int semester, string department, string catalog) {
            return await GetCourses(new CourseModel() {
                CatalogNumber = catalog,
                Department = department,
                Semester = semester
            });
        }

        private async Task<List<CourseModel>> GetCourses(CourseModel query) {
            var response = await _client.PostAsync(_configuration["enrollment_url"] + "?strm=" + query.Semester);
        }
    }
}