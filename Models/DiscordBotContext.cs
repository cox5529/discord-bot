using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

public class DiscordBotContext : DbContext {

    public DbSet<User> Users;
    public DbSet<CourseWatch> Watches;
    public DbSet<Enrollment> Entrollments;

    private readonly IConfigurationRoot _configuration;

    public DiscordBotContext(IConfigurationRoot configuration) {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseMySql(_configuration["connection_string"]);
        base.OnConfiguring(optionsBuilder);
    }
}

public class User {
    public int UserId { get; set; }
    public string DiscordId { get; set; }
    public virtual ICollection<CourseWatch> Watches { get; set; }
}

public class CourseWatch {
    public int CourseWatchId { get; set; }
    public string CourseName { get; set; }
    public string Department { get; set; }
    public string CatalogNumber { get; set; }
    public string SectionNumber { get; set; }
    public int UserId { get; set; }

    public virtual User User { get; set; }
    public virtual ICollection<Enrollment> Enrollments { get; set; }
}

public class Enrollment {
    public int EnrollmentId { get; set; }
    public int Total { get; set; }
    public int Max { get; set; }
    public DateTime Date { get; set; }
    public int CourseWatchId { get; set; }
    public virtual CourseWatch CourseWatch { get; set; }

}