using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CourseService.Services;
using Shared.Models;
using System.Threading.Tasks;

namespace CourseService.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseModel<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? fields = null, [FromQuery] string? expand = null)
        {
            var result = await _courseService.GetCoursesAsync(search, sort, page, size, fields, expand);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResponseModel<CourseResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? expand = null)
        {
            var result = await _courseService.GetCourseByIdAsync(id, expand);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id:int}/enrollments")]
        [ProducesResponseType(typeof(PagedResponseModel<EnrollmentResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEnrollmentsByCourse([FromServices] IEnrollmentService enrollmentService, int id, [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? expand = null)
        {
            var result = await enrollmentService.GetEnrollmentsAsync(null, id, null, null, page, size, null, expand);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<CourseResponseModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CourseRequestModel model)
        {
            var result = await _courseService.CreateCourseAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.CourseId }, result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ResponseModel<CourseResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] CourseRequestModel model)
        {
            var result = await _courseService.UpdateCourseAsync(id, model);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}




