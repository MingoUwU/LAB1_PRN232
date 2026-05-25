using PRN232.LMS.Services.Models;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IStudentService
    {
        Task<PagedResponseModel<object>> GetStudentsAsync(string? search, string? sort, int page, int size, string? fields);
        Task<ResponseModel<StudentResponseModel>> GetStudentByIdAsync(int id);
        Task<ResponseModel<StudentResponseModel>> CreateStudentAsync(StudentRequestModel model);
        Task<ResponseModel<StudentResponseModel>> UpdateStudentAsync(int id, StudentRequestModel model);
        Task<ResponseModel<bool>> DeleteStudentAsync(int id);
    }
}
