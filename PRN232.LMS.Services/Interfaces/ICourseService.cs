using PRN232.LMS.Services.Models;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PagedResponseModel<object>> GetCoursesAsync(string? search, string? sort, int page, int size, string? fields, string? expand);
        Task<ResponseModel<CourseResponseModel>> GetCourseByIdAsync(int id, string? expand = null);
        Task<ResponseModel<CourseResponseModel>> CreateCourseAsync(CourseRequestModel model);
        Task<ResponseModel<CourseResponseModel>> UpdateCourseAsync(int id, CourseRequestModel model);
        Task<ResponseModel<bool>> DeleteCourseAsync(int id);
    }
}
