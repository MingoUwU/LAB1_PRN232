using Microsoft.EntityFrameworkCore;
using CourseService.Entities;
using System;
using System.Collections.Generic;

namespace CourseService.Data
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options)
        {
        }

        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(e => e.SemesterId);
                entity.Property(e => e.SemesterName).HasMaxLength(100);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);
                entity.Property(e => e.CourseName).HasMaxLength(100);
                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.SemesterId);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.SubjectId);
                entity.Property(e => e.SubjectCode).HasMaxLength(20);
                entity.Property(e => e.SubjectName).HasMaxLength(100);
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.CourseId);
            });

            var semesters = new List<Semester>();
            for (int i = 1; i <= 5; i++)
            {
                semesters.Add(new Semester { SemesterId = i, SemesterName = $"Semester {i}", StartDate = DateTime.Now.AddMonths((i-1)*6), EndDate = DateTime.Now.AddMonths(i*6) });
            }
            modelBuilder.Entity<Semester>().HasData(semesters);

            var subjects = new List<Subject>();
            for (int i = 1; i <= 10; i++)
            {
                subjects.Add(new Subject { SubjectId = i, SubjectCode = $"SUB{i:00}", SubjectName = $"Subject {i}", Credit = 3 });
            }
            modelBuilder.Entity<Subject>().HasData(subjects);

            var courses = new List<Course>();
            for (int i = 1; i <= 20; i++)
            {
                courses.Add(new Course { CourseId = i, CourseName = $"Course {i}", SemesterId = (i % 5) + 1 });
            }
            modelBuilder.Entity<Course>().HasData(courses);

            var enrollments = new List<Enrollment>();
            for (int i = 1; i <= 500; i++)
            {
                enrollments.Add(new Enrollment { EnrollmentId = i, StudentId = ((i - 1) % 50) + 1, CourseId = ((i - 1) % 20) + 1, EnrollDate = DateTime.Now, Status = "Active" });
            }
            modelBuilder.Entity<Enrollment>().HasData(enrollments);
        }
    }
}
