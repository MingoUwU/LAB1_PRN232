using Shared.Models;
using System.Threading.Tasks;

namespace CourseService.Services
{
    public interface IEnrollmentService
    {
        Task<PagedResponseModel<EnrollmentResponseModel>> GetEnrollmentsAsync(int? studentId, int? courseId, string? search, string? sort, int page, int size, string? fields, string? expand);
        Task<ResponseModel<EnrollmentResponseModel>> GetEnrollmentByIdAsync(int id);
        Task<ResponseModel<EnrollmentResponseModel>> CreateEnrollmentAsync(EnrollmentRequestModel model);
        Task<ResponseModel<EnrollmentResponseModel>> UpdateEnrollmentAsync(int id, EnrollmentRequestModel model);
        Task<ResponseModel<bool>> DeleteEnrollmentAsync(int id);
    }
}
