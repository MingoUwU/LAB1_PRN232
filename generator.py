import os

base_dir = r"d:\PRN232\LAB1"

def gen_service(entity_name, var_name, search_cond, request_model, response_model):
    return f"""using AutoMapper;
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
{{
    public class {entity_name}Service : I{entity_name}Service
    {{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public {entity_name}Service(IUnitOfWork unitOfWork, IMapper mapper)
        {{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }}

        public async Task<PagedResponseModel<object>> Get{entity_name}sAsync(string? search, string? sort, int page, int size, string? fields)
        {{
            var query = _unitOfWork.{entity_name}s.GetQueryable();

            if (!string.IsNullOrEmpty(search))
            {{
                query = query.Where(c => {search_cond});
            }}

            if (!string.IsNullOrEmpty(sort))
            {{
                var sortParams = sort.Split(',');
                var sortString = "";
                foreach (var param in sortParams)
                {{
                    if (param.StartsWith("-")) sortString += param.Substring(1) + " descending, ";
                    else sortString += param + " ascending, ";
                }}
                sortString = sortString.TrimEnd(',', ' ');
                if (!string.IsNullOrEmpty(sortString)) query = query.OrderBy(sortString);
            }}

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            var dtos = _mapper.Map<IEnumerable<{response_model}>>(items).ToList();
            var shapedData = new List<object>();

            if (!string.IsNullOrWhiteSpace(fields))
            {{
                var selectedFields = fields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToList();
                foreach (var dto in dtos)
                {{
                    var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object?>;
                    var type = typeof({response_model});
                    foreach (var field in selectedFields)
                    {{
                        var property = type.GetProperties(System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));
                        if (property != null)
                        {{
                            expando.Add(char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1), property.GetValue(dto));
                        }}
                    }}
                    shapedData.Add(expando);
                }}
            }}
            else shapedData.AddRange(dtos);

            return new PagedResponseModel<object>
            {{
                Success = true, Message = "Retrieved successfully", Data = shapedData,
                Pagination = new PaginationMetadata {{ Page = page, PageSize = size, TotalItems = totalItems, TotalPages = totalPages }}
            }};
        }}

        public async Task<ResponseModel<{response_model}>> Get{entity_name}ByIdAsync(int id)
        {{
            var {var_name} = await _unitOfWork.{entity_name}s.GetByIdAsync(id);
            if ({var_name} == null) return new ResponseModel<{response_model}> {{ Success = false, Message = "Not found", Errors = new List<string> {{ "Not Found" }} }};
            return new ResponseModel<{response_model}> {{ Success = true, Data = _mapper.Map<{response_model}>({var_name}) }};
        }}

        public async Task<ResponseModel<{response_model}>> Create{entity_name}Async({request_model} model)
        {{
            var {var_name} = _mapper.Map<{entity_name}>(model);
            await _unitOfWork.{entity_name}s.AddAsync({var_name});
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel<{response_model}> {{ Success = true, Message = "Created", Data = _mapper.Map<{response_model}>({var_name}) }};
        }}

        public async Task<ResponseModel<{response_model}>> Update{entity_name}Async(int id, {request_model} model)
        {{
            var {var_name} = await _unitOfWork.{entity_name}s.GetByIdAsync(id);
            if ({var_name} == null) return new ResponseModel<{response_model}> {{ Success = false, Message = "Not found", Errors = new List<string> {{ "Not Found" }} }};
            _mapper.Map(model, {var_name});
            _unitOfWork.{entity_name}s.Update({var_name});
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel<{response_model}> {{ Success = true, Message = "Updated", Data = _mapper.Map<{response_model}>({var_name}) }};
        }}

        public async Task<ResponseModel<bool>> Delete{entity_name}Async(int id)
        {{
            var {var_name} = await _unitOfWork.{entity_name}s.GetByIdAsync(id);
            if ({var_name} == null) return new ResponseModel<bool> {{ Success = false, Message = "Not found", Data = false, Errors = new List<string> {{ "Not Found" }} }};
            _unitOfWork.{entity_name}s.Delete({var_name});
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel<bool> {{ Success = true, Message = "Deleted", Data = true }};
        }}
    }}
}}
"""

def gen_controller(entity_name, var_name, request_model, response_model):
    return f"""using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Controllers
{{
    [ApiController]
    [Route("api/[controller]")]
    public class {entity_name}sController : ControllerBase
    {{
        private readonly I{entity_name}Service _{var_name}Service;

        public {entity_name}sController(I{entity_name}Service {var_name}Service)
        {{
            _{var_name}Service = {var_name}Service;
        }}

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseModel<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? fields = null)
        {{
            var result = await _{var_name}Service.Get{entity_name}sAsync(search, sort, page, size, fields);
            return Ok(result);
        }}

        [HttpGet("{{id}}")]
        [ProducesResponseType(typeof(ResponseModel<{response_model}>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id)
        {{
            var result = await _{var_name}Service.Get{entity_name}ByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }}

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<{response_model}>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] {request_model} model)
        {{
            var result = await _{var_name}Service.Create{entity_name}Async(model);
            return CreatedAtAction(nameof(GetById), new {{ id = result.Data?.{entity_name}Id }}, result);
        }}

        [HttpPut("{{id}}")]
        [ProducesResponseType(typeof(ResponseModel<{response_model}>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] {request_model} model)
        {{
            var result = await _{var_name}Service.Update{entity_name}Async(id, model);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }}

        [HttpDelete("{{id}}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {{
            var result = await _{var_name}Service.Delete{entity_name}Async(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }}
    }}
}}
"""

services = {
    "Course": gen_service("Course", "course", "c.CourseName.Contains(search)", "CourseRequestModel", "CourseResponseModel"),
    "Semester": gen_service("Semester", "semester", "c.SemesterName.Contains(search)", "SemesterRequestModel", "SemesterResponseModel"),
    "Subject": gen_service("Subject", "subject", "c.SubjectName.Contains(search) || c.SubjectCode.Contains(search)", "SubjectRequestModel", "SubjectResponseModel")
}

controllers = {
    "Course": gen_controller("Course", "course", "CourseRequestModel", "CourseResponseModel"),
    "Semester": gen_controller("Semester", "semester", "SemesterRequestModel", "SemesterResponseModel"),
    "Subject": gen_controller("Subject", "subject", "SubjectRequestModel", "SubjectResponseModel")
}

for k, v in services.items():
    with open(os.path.join(base_dir, f"PRN232.LMS.Services\Implementations\{k}Service.cs"), "w", encoding="utf-8") as f:
        f.write(v)

for k, v in controllers.items():
    with open(os.path.join(base_dir, f"PRN232.LMS.API\Controllers\{k}sController.cs"), "w", encoding="utf-8") as f:
        f.write(v)

print("Done generating")
