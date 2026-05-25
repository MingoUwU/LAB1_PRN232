using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponseModel<EnrollmentResponseModel>> GetEnrollmentsAsync(int? studentId, string? search, string? sort, int page, int size, string? fields, string? expand)
        {
            var query = _unitOfWork.Enrollments.GetQueryable();

            if (studentId.HasValue)
            {
                query = query.Where(e => e.StudentId == studentId.Value);
            }

            if (!string.IsNullOrEmpty(expand))
            {
                var expansions = expand.Split(',');
                foreach (var exp in expansions)
                {
                    if (exp.Equals("student", StringComparison.OrdinalIgnoreCase))
                        query = query.Include(e => e.Student);
                    if (exp.Equals("course", StringComparison.OrdinalIgnoreCase))
                        query = query.Include(e => e.Course);
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Status.Contains(search));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                var sortParams = sort.Split(',');
                var sortString = "";
                foreach (var param in sortParams)
                {
                    if (param.StartsWith("-"))
                        sortString += param.Substring(1) + " descending, ";
                    else
                        sortString += param + " ascending, ";
                }
                sortString = sortString.TrimEnd(',', ' ');
                if (!string.IsNullOrEmpty(sortString))
                    query = query.OrderBy(sortString);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);

            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            var dtos = _mapper.Map<IEnumerable<EnrollmentResponseModel>>(items).ToList();

            // Đảm bảo tuân thủ TUYỆT ĐỐI yêu cầu của đề bài: 
            // Chỉ trả về object Student/Course chi tiết nếu người dùng gõ chữ "expand".
            bool expandStudent = !string.IsNullOrEmpty(expand) && expand.Contains("student", StringComparison.OrdinalIgnoreCase);
            bool expandCourse = !string.IsNullOrEmpty(expand) && expand.Contains("course", StringComparison.OrdinalIgnoreCase);

            foreach (var dto in dtos)
            {
                if (!expandStudent) dto.Student = null;
                if (!expandCourse) dto.Course = null;
            }

            return new PagedResponseModel<EnrollmentResponseModel>
            {
                Success = true,
                Message = "Enrollments retrieved successfully",
                Data = dtos,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = size,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<ResponseModel<EnrollmentResponseModel>> GetEnrollmentByIdAsync(int id)
        {
            var enrollment = await _unitOfWork.Enrollments.GetQueryable()
                                .Include(e => e.Student)
                                .Include(e => e.Course)
                                .FirstOrDefaultAsync(e => e.EnrollmentId == id);
            
            if (enrollment == null)
                return new ResponseModel<EnrollmentResponseModel> { Success = false, Message = "Not found", Errors = new List<string> { "Enrollment not found" } };

            var dto = _mapper.Map<EnrollmentResponseModel>(enrollment);
            return new ResponseModel<EnrollmentResponseModel> { Success = true, Message = "Retrieved successfully", Data = dto };
        }

        public async Task<ResponseModel<EnrollmentResponseModel>> CreateEnrollmentAsync(EnrollmentRequestModel model)
        {
            var enrollment = _mapper.Map<Enrollment>(model);
            await _unitOfWork.Enrollments.AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<EnrollmentResponseModel>(enrollment);
            return new ResponseModel<EnrollmentResponseModel> { Success = true, Message = "Created successfully", Data = dto };
        }

        public async Task<ResponseModel<EnrollmentResponseModel>> UpdateEnrollmentAsync(int id, EnrollmentRequestModel model)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                return new ResponseModel<EnrollmentResponseModel> { Success = false, Message = "Not found", Errors = new List<string> { "Not found" } };

            _mapper.Map(model, enrollment);
            _unitOfWork.Enrollments.Update(enrollment);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<EnrollmentResponseModel>(enrollment);
            return new ResponseModel<EnrollmentResponseModel> { Success = true, Message = "Updated successfully", Data = dto };
        }

        public async Task<ResponseModel<bool>> DeleteEnrollmentAsync(int id)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                return new ResponseModel<bool> { Success = false, Message = "Not found", Data = false, Errors = new List<string> { "Not found" } };

            _unitOfWork.Enrollments.Delete(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseModel<bool> { Success = true, Message = "Deleted successfully", Data = true };
        }
    }
}
