using Grpc.Core;
using StudentService.Grpc;
using StudentService.Repositories;
using System.Threading.Tasks;

namespace StudentService.Services
{
    public class StudentGrpcService : StudentGrpc.StudentGrpcBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentGrpcService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<StudentExistsResponse> CheckStudentExists(StudentExistsRequest request, ServerCallContext context)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId);
            return new StudentExistsResponse
            {
                Exists = student != null
            };
        }
    }
}
