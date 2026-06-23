using Microsoft.EntityFrameworkCore;
using StudentService.Entities;
using System;
using System.Collections.Generic;

namespace StudentService.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
            });

            var students = new List<Student>();
            for (int i = 1; i <= 50; i++)
            {
                students.Add(new Student { StudentId = i, FullName = $"Student {i}", Email = $"student{i}@test.com", DateOfBirth = new DateTime(2000, 1, 1).AddDays(i) });
            }
            modelBuilder.Entity<Student>().HasData(students);
        }
    }
}
