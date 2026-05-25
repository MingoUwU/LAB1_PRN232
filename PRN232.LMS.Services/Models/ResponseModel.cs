using System.Collections.Generic;

namespace PRN232.LMS.Services.Models
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }

    public class PaginationMetadata
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class PagedResponseModel<T> : ResponseModel<IEnumerable<T>>
    {
        public PaginationMetadata Pagination { get; set; } = null!;
    }
}
