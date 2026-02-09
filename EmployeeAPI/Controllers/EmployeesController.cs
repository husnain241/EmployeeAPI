using EmployeeAPI.Dtos;
using EmployeeAPI.Models;
using EmployeeAPI.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace EmployeeAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;
        public EmployeesController(IEmployeeRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetEmployees(
            [FromQuery] string? name, [FromQuery] string? department, int pageNumber = 1, int pageSize = 10)
        {
            return Ok(await _repository.GetAllAsync(name, department, pageNumber, pageSize));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeReadDto>> GetEmployee(int id)
        {
            var result = await _repository.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeReadDto>> CreateEmployee([FromBody] EmployeeCreateDto dto)
        {
            var result = await _repository.AddAsync(dto);
            return CreatedAtAction(nameof(GetEmployee), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto dto)
        {
            var success = await _repository.UpdateAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var success = await _repository.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
