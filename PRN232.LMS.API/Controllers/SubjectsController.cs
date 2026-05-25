using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseModel<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? fields = null)
        {
            var result = await _subjectService.GetSubjectsAsync(search, sort, page, size, fields);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<SubjectResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _subjectService.GetSubjectByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<SubjectResponseModel>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] SubjectRequestModel model)
        {
            var result = await _subjectService.CreateSubjectAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.SubjectId }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<SubjectResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] SubjectRequestModel model)
        {
            var result = await _subjectService.UpdateSubjectAsync(id, model);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _subjectService.DeleteSubjectAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
