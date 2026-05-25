using System;

namespace PRN232.LMS.Services.Models
{
    public class EnrollmentBusinessModel
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;
    }

    public class EnrollmentRequestModel
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;
    }

    public class EnrollmentResponseModel : EnrollmentBusinessModel
    {
        [System.Text.Json.Serialization.JsonPropertyOrder(98)]
        public StudentResponseModel? Student { get; set; }

        [System.Text.Json.Serialization.JsonPropertyOrder(99)]
        public CourseResponseModel? Course { get; set; }
    }
}
