using Shared.Models;
using System.Threading.Tasks;

namespace CourseService.Services
{
    public interface ISemesterService
    {
        Task<PagedResponseModel<object>> GetSemestersAsync(string? search, string? sort, int page, int size, string? fields);
        Task<ResponseModel<SemesterResponseModel>> GetSemesterByIdAsync(int id);
        Task<ResponseModel<SemesterResponseModel>> CreateSemesterAsync(SemesterRequestModel model);
        Task<ResponseModel<SemesterResponseModel>> UpdateSemesterAsync(int id, SemesterRequestModel model);
        Task<ResponseModel<bool>> DeleteSemesterAsync(int id);
    }
}
