using System;

namespace Shared.Models
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
        [System.Text.Json.Serialization.JsonPropertyOrder(99)]
        public SemesterResponseModel? Semester { get; set; }
    }
}
