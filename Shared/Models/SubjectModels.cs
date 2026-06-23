namespace Shared.Models
{
    public class SubjectBusinessModel
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credit { get; set; }
    }

    public class SubjectRequestModel
    {
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credit { get; set; }
    }

    public class SubjectResponseModel : SubjectBusinessModel
    {
    }
}
