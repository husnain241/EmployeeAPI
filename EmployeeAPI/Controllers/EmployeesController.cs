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

        public EmployeesController(IEmployeeRepository repository) =>
            _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetEmployees(
            [FromQuery] string? name, [FromQuery] string? department)
        {
            var employees = await _repository.GetAllAsync(name, department);
            var dtos = employees.Select(e => new EmployeeReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Department = e.Department,
                Age = e.Age,
                Email = e.Email,
                Detail = e.Detail // Default detail

            });

            return Ok(dtos); // 200
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeReadDto>> GetEmployee(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null)
                return NotFound(); // 404

            var dto = new EmployeeReadDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Department = employee.Department,
                Age = employee.Age,
                Email = employee.Email
            };

            return Ok(dto); // 200
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeReadDto>> CreateEmployee(
            [FromBody] EmployeeCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            var employee = new Employee
            {
                Name = dto.Name,
                Department = dto.Department,
                Age = dto.Age,
                Email = dto.Email,
            };

            await _repository.AddAsync(employee);
            await _repository.SaveChangesAsync();

            var readDto = new EmployeeReadDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Department = employee.Department,
                Age = employee.Age,
                Email = employee.Email
            };

            return CreatedAtAction(nameof(GetEmployee),
                new { id = employee.Id }, readDto); // 201
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmployee(
            int id, [FromBody] EmployeeUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            var employee = await _repository.GetByIdAsync(id);
            if (employee == null)
                return NotFound(); // 404

            employee.Name = dto.Name;
            employee.Department = dto.Department;
            employee.Age = dto.Age;
            employee.Email = dto.Email;

            _repository.Update(employee);
            await _repository.SaveChangesAsync();

            return NoContent(); // 204
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null)
                return NotFound(); // 404

            _repository.Remove(employee);
            await _repository.SaveChangesAsync();

            return NoContent(); // 204
        }

       

    }
}
