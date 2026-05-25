using PRN232.LMS.Services.Models;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
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
