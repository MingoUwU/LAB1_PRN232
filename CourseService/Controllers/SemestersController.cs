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
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseModel<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? fields = null)
        {
            var result = await _semesterService.GetSemestersAsync(search, sort, page, size, fields);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResponseModel<SemesterResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _semesterService.GetSemesterByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<SemesterResponseModel>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] SemesterRequestModel model)
        {
            var result = await _semesterService.CreateSemesterAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.SemesterId }, result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ResponseModel<SemesterResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] SemesterRequestModel model)
        {
            var result = await _semesterService.UpdateSemesterAsync(id, model);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _semesterService.DeleteSemesterAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}




