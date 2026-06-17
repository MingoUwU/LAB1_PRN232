using System;

namespace PRN232.LMS.Services.Models
{
    public class StudentBusinessModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? StudentCode { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class StudentRequestModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = null!;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = null!;

        [System.ComponentModel.DataAnnotations.Phone]
        public string? PhoneNumber { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.RegularExpression(@"^(SE|CE|ME|HE|SA)\d{5}$", ErrorMessage = "Student code must be in FPTU style (e.g., SE19886)")]
        public string StudentCode { get; set; } = null!;

        [System.ComponentModel.DataAnnotations.Range(typeof(DateTime), "1/1/1900", "1/1/2100")]
        public DateTime DateOfBirth { get; set; }
    }
    
    public class StudentResponseModel : StudentBusinessModel
    {
    }
}
