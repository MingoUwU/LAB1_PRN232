using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentService.Services;
using Shared.Models;
using System.Threading.Tasks;

namespace StudentService.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(PagedResponseModel<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromHeader(Name = "X-Request-Id")] string? requestId, [FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? fields = null)
        {
            var result = await _studentService.GetStudentsAsync(search, sort, page, size, fields);
            if (!string.IsNullOrEmpty(requestId)) Response.Headers["X-Request-Id"] = requestId;
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetStudentById")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _studentService.GetStudentByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }



        [HttpPost]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] StudentRequestModel model)
        {
            var result = await _studentService.CreateStudentAsync(model);
            return CreatedAtRoute("GetStudentById", new { id = result.Data?.StudentId, version = "1.0" }, result);
        }

        [HttpPut("{id:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] StudentRequestModel model)
        {
            var result = await _studentService.UpdateStudentAsync(id, model);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    
        [HttpGet]
        [MapToApiVersion("2.0")]
        [Authorize]
        [ProducesResponseType(typeof(PagedResponseModel<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetV2([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? fields = null)
        {
            var result = await _studentService.GetStudentsAsync(search, sort, page, size, fields);
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetStudentByIdV2")]
        [MapToApiVersion("2.0")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByIdV2(int id)
        {
            var result = await _studentService.GetStudentByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateV2([FromBody] StudentRequestModel model)
        {
            var result = await _studentService.CreateStudentAsync(model);
            return CreatedAtRoute("GetStudentByIdV2", new { id = result.Data?.StudentId, version = "2.0" }, result);
        }
    }
}
