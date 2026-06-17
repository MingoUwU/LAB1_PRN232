using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using System;
using System.Collections.Generic;

namespace PRN232.LMS.Repositories
{
    public class LmsDbContext : DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options)
        {
        }

        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

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

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.StudentId);
                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.CourseId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).HasMaxLength(50);
                entity.Property(e => e.PasswordHash).HasMaxLength(255);
                entity.Property(e => e.Role).HasMaxLength(20);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
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

            var students = new List<Student>();
            for (int i = 1; i <= 50; i++)
            {
                students.Add(new Student { StudentId = i, FullName = $"Student {i}", Email = $"student{i}@test.com", DateOfBirth = new DateTime(2000, 1, 1).AddDays(i) });
            }
            modelBuilder.Entity<Student>().HasData(students);

            var enrollments = new List<Enrollment>();
            for (int i = 1; i <= 500; i++)
            {
                enrollments.Add(new Enrollment { EnrollmentId = i, StudentId = ((i - 1) % 50) + 1, CourseId = ((i - 1) % 20) + 1, EnrollDate = DateTime.Now, Status = "Active" });
            }
            modelBuilder.Entity<Enrollment>().HasData(enrollments);

            // Seed Admin User (Password: 123456)
            // PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456");
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    PasswordHash = "$2a$11$BAw1wi3f60j7x2XzbChWpeZtLHjRNbToBWVMtsD.JQGargUDYmADG", // 123456 using bcrypt
                    Role = "Admin"
                }
            );
        }
    }
}
