using System;

namespace Shared.Models
{
    public class SemesterBusinessModel
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SemesterRequestModel
    {
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SemesterResponseModel : SemesterBusinessModel
    {
    }
}
