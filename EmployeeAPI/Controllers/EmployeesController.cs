using EmployeeAPI.Dtos;
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
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string? name,
            [FromQuery] string? department,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _repository.GetAllAsync(name, department, pageNumber, pageSize);
            // Get mein aksar hum direct result bhej dete hain kyunki ye fail kam hota hai
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result); // 404 with error message in Result object

            return Ok(result); // 200 with data in Result object
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _repository.AddAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result); // e.g., Email already exists

            return CreatedAtAction(nameof(GetEmployee), new { id = result.Data!.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _repository.UpdateAsync(id, dto);

            if (!result.IsSuccess)
                return NotFound(result); // Record not found or update failed

            return Ok(result); // Noocntent ki bajaye Result bhej rahe hain taake message mil sake
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result); // Result object with success message
        }
    }
}