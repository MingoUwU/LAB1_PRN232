using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseModel<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? fields = null)
        {
            var result = await _studentService.GetStudentsAsync(search, sort, page, size, fields);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _studentService.GetStudentByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id}/enrollments")]
        [ProducesResponseType(typeof(PagedResponseModel<EnrollmentResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEnrollmentsByStudent([FromServices] IEnrollmentService enrollmentService, int id, [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? expand = null)
        {
            var result = await enrollmentService.GetEnrollmentsAsync(id, null, null, page, size, null, expand);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] StudentRequestModel model)
        {
            var result = await _studentService.CreateStudentAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.StudentId }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<StudentResponseModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] StudentRequestModel model)
        {
            var result = await _studentService.UpdateStudentAsync(id, model);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
