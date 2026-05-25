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
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponseModel<object>> GetCoursesAsync(string? search, string? sort, int page, int size, string? fields)
        {
            var query = _unitOfWork.Courses.GetQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CourseName.Contains(search));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                var sortParams = sort.Split(',');
                var sortString = "";
                foreach (var param in sortParams)
                {
                    if (param.StartsWith("-")) sortString += param.Substring(1) + " descending, ";
                    else sortString += param + " ascending, ";
                }
                sortString = sortString.TrimEnd(',', ' ');
                if (!string.IsNullOrEmpty(sortString)) query = query.OrderBy(sortString);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            var dtos = _mapper.Map<IEnumerable<CourseResponseModel>>(items).ToList();
            var shapedData = new List<object>();

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var selectedFields = fields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToList();
                foreach (var dto in dtos)
                {
                    var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object?>;
                    var type = typeof(CourseResponseModel);
                    foreach (var field in selectedFields)
                    {
                        var property = type.GetProperties(System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));
                        if (property != null)
                        {
                            expando.Add(char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1), property.GetValue(dto));
                        }
                    }
                    shapedData.Add(expando);
                }
            }
            else shapedData.AddRange(dtos);

            return new PagedResponseModel<object>
            {
                Success = true, Message = "Retrieved successfully", Data = shapedData,
                Pagination = new PaginationMetadata { Page = page, PageSize = size, TotalItems = totalItems, TotalPages = totalPages }
            };
        }

        public async Task<ResponseModel<CourseResponseModel>> GetCourseByIdAsync(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null) return new ResponseModel<CourseResponseModel> { Success = false, Message = "Not found", Errors = new List<string> { "Not Found" } };
            return new ResponseModel<CourseResponseModel> { Success = true, Data = _mapper.Map<CourseResponseModel>(course) };
        }

        public async Task<ResponseModel<CourseResponseModel>> CreateCourseAsync(CourseRequestModel model)
        {
            var course = _mapper.Map<Course>(model);
            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel<CourseResponseModel> { Success = true, Message = "Created", Data = _mapper.Map<CourseResponseModel>(course) };
        }

        public async Task<ResponseModel<CourseResponseModel>> UpdateCourseAsync(int id, CourseRequestModel model)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null) return new ResponseModel<CourseResponseModel> { Success = false, Message = "Not found", Errors = new List<string> { "Not Found" } };
            _mapper.Map(model, course);
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel<CourseResponseModel> { Success = true, Message = "Updated", Data = _mapper.Map<CourseResponseModel>(course) };
        }

        public async Task<ResponseModel<bool>> DeleteCourseAsync(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null) return new ResponseModel<bool> { Success = false, Message = "Not found", Data = false, Errors = new List<string> { "Not Found" } };
            _unitOfWork.Courses.Delete(course);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel<bool> { Success = true, Message = "Deleted", Data = true };
        }
    }
}
