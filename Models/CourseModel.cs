using System;
using System.Collections.Generic;

namespace Discord_bot.Models {

    public class CourseModel {
        public string Name { get; set; }
        public int Semester { get; set; }
        public string Instructor { get; set; }
        public string Department { get; set; }
        public string CatalogNumber { get; set; }
        public string SectionNumber { get; set; }
        public string UAConnectId { get; set; }
        public ICollection<DayOfWeek> Days { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Enrolled { get; set; }
        public int Size { get; set; }
        public string Location { get; set; }
    }

    public enum DayOfWeek {
        Monday, Tuesday, Wednesday, Thursday, Friday
    }
}