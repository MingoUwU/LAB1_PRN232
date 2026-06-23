using System;
using System.Collections.Generic;

namespace StudentService.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

    }
}
