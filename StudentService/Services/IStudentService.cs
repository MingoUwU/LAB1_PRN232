using Shared.Models;
using System.Threading.Tasks;

namespace StudentService.Services
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
