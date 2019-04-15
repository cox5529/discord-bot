using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord_bot.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Discord_bot.Services {
    public class EnrollmentService {
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
            var client = new RestClient("https://scheduleofclasses.uark.edu/Main?strm=1199");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("undefined", $"campus=FAY&subject={query.Department}&catalog_nbr1={query.CatalogNumber}&Search=Search",
                ParameterType.RequestBody);
            var response = await client.ExecutePostTaskAsync(request);
            var doc = new HtmlDocument();
            doc.LoadHtml(response.Content);

            var rows = doc.DocumentNode.SelectNodes("//div[@id='table']/table/tr");

            var courses = new List<CourseModel>();
            foreach (var row in rows) {
                if (row.SelectSingleNode("th") != null) continue;
                var courseDescription = row.SelectSingleNode("td[@class='CourseID']").InnerText.Split(' ');
                var enrollmentString = row.SelectSingleNode("td[@class='EnrolledSize']").InnerText.Split('/');
                if (rows.Count > 10 && (courseDescription[1].StartsWith("L0") || courseDescription[0].EndsWith("V") ||
                                        courseDescription[0].EndsWith("VH"))) {
                    continue;
                }

                var course = new CourseModel() {
                    Department = courseDescription[0].Substring(0, 4),
                    CatalogNumber = courseDescription[0].Substring(4),
                    Enrolled = int.Parse(enrollmentString[0]),
                    Size = int.Parse(enrollmentString[1]),
                    Instructor = row.SelectSingleNode("td[@class='Instructor']").InnerText,
                    SectionNumber = courseDescription[1],
                    Name = row.SelectSingleNode("td[@class='Title']").InnerText
                };
                courses.Add(course);
            }

            return courses;
        }
    }
}