using Shared.Models;
using System.Threading.Tasks;

namespace CourseService.Services
{
    public interface ISubjectService
    {
        Task<PagedResponseModel<object>> GetSubjectsAsync(string? search, string? sort, int page, int size, string? fields);
        Task<ResponseModel<SubjectResponseModel>> GetSubjectByIdAsync(int id);
        Task<ResponseModel<SubjectResponseModel>> CreateSubjectAsync(SubjectRequestModel model);
        Task<ResponseModel<SubjectResponseModel>> UpdateSubjectAsync(int id, SubjectRequestModel model);
        Task<ResponseModel<bool>> DeleteSubjectAsync(int id);
    }
}
