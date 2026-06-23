using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CourseService.Entities;
using CourseService.Repositories;
using CourseService.Services;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace CourseService.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponseModel<object>> GetSubjectsAsync(string? search, string? sort, int page, int size, string? fields)
        {
            var query = _unitOfWork.Subjects.GetQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.SubjectName.Contains(search) || c.SubjectCode.Contains(search));
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
            var dtos = _mapper.Map<IEnumerable<SubjectResponseModel>>(items).ToList();
            var shapedData = new List<object>();

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var selectedFields = fields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToList();
                foreach (var dto in dtos)
                {
                    var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object?>;
                    var type = typeof(SubjectResponseModel);
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

        public async Task<ResponseModel<SubjectResponseModel>> GetSubjectByIdAsync(int id)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
            if (subject == null) return new ResponseModel<SubjectResponseModel> { Success = false, Message = "Not found", Errors = new List<string> { "Not Found" } };
            return new ResponseModel<SubjectResponseModel> { Success = true, Data = _mapper.Map<SubjectResponseModel>(subject) };
        }

        public async Task<ResponseModel<SubjectResponseModel>> CreateSubjectAsync(SubjectRequestModel model)
        {
            var subject = _mapper.Map<Subject>(model);
            await _unitOfWork.Subjects.AddAsync(subject);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel<SubjectResponseModel> { Success = true, Message = "Created", Data = _mapper.Map<SubjectResponseModel>(subject) };
        }

        public async Task<ResponseModel<SubjectResponseModel>> UpdateSubjectAsync(int id, SubjectRequestModel model)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
            if (subject == null) return new ResponseModel<SubjectResponseModel> { Success = false, Message = "Not found", Errors = new List<string> { "Not Found" } };
            _mapper.Map(model, subject);
            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel<SubjectResponseModel> { Success = true, Message = "Updated", Data = _mapper.Map<SubjectResponseModel>(subject) };
        }

        public async Task<ResponseModel<bool>> DeleteSubjectAsync(int id)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
            if (subject == null) return new ResponseModel<bool> { Success = false, Message = "Not found", Data = false, Errors = new List<string> { "Not Found" } };
            _unitOfWork.Subjects.Delete(subject);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel<bool> { Success = true, Message = "Deleted", Data = true };
        }
    }
}
