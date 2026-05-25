using System;

namespace PRN232.LMS.Services.Models
{
    public class CourseBusinessModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
    }

    public class CourseRequestModel
    {
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
    }

    public class CourseResponseModel : CourseBusinessModel
    {
    }
}
