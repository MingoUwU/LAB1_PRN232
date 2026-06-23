using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentService.Entities;
using StudentService.Repositories;
using StudentService.Services;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace StudentService.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponseModel<object>> GetStudentsAsync(string? search, string? sort, int page, int size, string? fields)
        {
            var query = _unitOfWork.Students.GetQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.FullName.Contains(search) || s.Email.Contains(search));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                var sortParams = sort.Split(',');
                var sortString = "";
                foreach (var param in sortParams)
                {
                    if (param.StartsWith("-"))
                    {
                        sortString += param.Substring(1) + " descending, ";
                    }
                    else
                    {
                        sortString += param + " ascending, ";
                    }
                }
                sortString = sortString.TrimEnd(',', ' ');
                if (!string.IsNullOrEmpty(sortString))
                {
                    query = query.OrderBy(sortString);
                }
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);

            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            var dtos = _mapper.Map<IEnumerable<StudentResponseModel>>(items).ToList();
            var shapedData = new List<object>();

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var selectedFields = fields.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(f => f.Trim())
                                           .ToList();

                foreach (var dto in dtos)
                {
                    var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object?>;
                    var type = typeof(StudentResponseModel);
                    foreach (var field in selectedFields)
                    {
                        var property = type.GetProperties(System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                           .FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));

                        if (property != null)
                        {
                            // Convert first character to lowercase for standard JSON camelCase
                            string camelCaseName = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
                            expando.Add(camelCaseName, property.GetValue(dto));
                        }
                    }
                    shapedData.Add(expando);
                }
            }
            else
            {
                shapedData.AddRange(dtos);
            }

            return new PagedResponseModel<object>
            {
                Success = true,
                Message = "Students retrieved successfully",
                Data = shapedData,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = size,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<ResponseModel<StudentResponseModel>> GetStudentByIdAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                return new ResponseModel<StudentResponseModel> { Success = false, Message = "Student not found", Errors = new List<string> { "Not Found" } };
            }

            var dto = _mapper.Map<StudentResponseModel>(student);
            return new ResponseModel<StudentResponseModel> { Success = true, Message = "Student retrieved successfully", Data = dto };
        }

        public async Task<ResponseModel<StudentResponseModel>> CreateStudentAsync(StudentRequestModel model)
        {
            var student = _mapper.Map<Student>(model);
            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<StudentResponseModel>(student);
            return new ResponseModel<StudentResponseModel> { Success = true, Message = "Student created successfully", Data = dto };
        }

        public async Task<ResponseModel<StudentResponseModel>> UpdateStudentAsync(int id, StudentRequestModel model)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                return new ResponseModel<StudentResponseModel> { Success = false, Message = "Student not found", Errors = new List<string> { "Not Found" } };
            }

            _mapper.Map(model, student);
            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<StudentResponseModel>(student);
            return new ResponseModel<StudentResponseModel> { Success = true, Message = "Student updated successfully", Data = dto };
        }

        public async Task<ResponseModel<bool>> DeleteStudentAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                return new ResponseModel<bool> { Success = false, Message = "Student not found", Data = false, Errors = new List<string> { "Not Found" } };
            }

            _unitOfWork.Students.Delete(student);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseModel<bool> { Success = true, Message = "Student deleted successfully", Data = true };
        }
    }
}
