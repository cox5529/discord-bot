using System;
using System.Collections.Generic;

namespace Discord_bot.Models {

    public class CourseModel {
        public string Name { get; set; } = "";
        public int Semester { get; set; } = 1199;
        public string Instructor { get; set; } = "";
        public string Department { get; set; } = "";
        public string CatalogNumber { get; set; } = "";
        public string SectionNumber { get; set; } = "";
        public string UAConnectId { get; set; } = "";
        public ICollection<DayOfWeek> Days { get; set; } = new List<DayOfWeek>();
        public DateTime Start { get; set; } = DateTime.MinValue;
        public DateTime End { get; set; } = DateTime.MinValue;
        public int Enrolled { get; set; } = 0;
        public int Size { get; set; } = 0;
        public string Location { get; set; } = "";
    }

    public enum DayOfWeek {
        Monday, Tuesday, Wednesday, Thursday, Friday
    }
}