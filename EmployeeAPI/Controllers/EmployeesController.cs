using EmployeeAPI.Common;
using EmployeeAPI.Dtos;
using EmployeeAPI.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly IValidator<EmployeeCreateDto> _createValidator; // Yeh line check karein
        public EmployeesController(IEmployeeRepository repository,IMemoryCache cache, IValidator<EmployeeCreateDto> createValidator)    
        {
            _repository = repository;
            _cache = cache;
            _createValidator = createValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(
     [FromQuery] string? name,
     [FromQuery] string? department,
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10)
        {
            // Unique key jo filter aur page par depend kare
            string cacheKey = $"emps_{name}_{department}_{pageNumber}_{pageSize}";

            if (!_cache.TryGetValue(cacheKey, out Result<IEnumerable<EmployeeReadDto>> cachedEmployees))
            {
                cachedEmployees = await _repository.GetAllAsync(name, department, pageNumber, pageSize);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60));

                _cache.Set(cacheKey, cachedEmployees, cacheOptions);
            }

            return Ok(cachedEmployees);
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
            // 1. Fluent Validation ko manually call karein
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                // Yahan 'Select' use karne se saare errors aik list mein aa jayenge
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errorMessages });
            }
            // 2. Baaki logic wahi rahegi
            var result = await _repository.AddAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            _cache.Remove("all_employees_data");

            return CreatedAtAction(nameof(GetEmployee), new { id = result.Data!.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _repository.UpdateAsync(id, dto);

            if (!result.IsSuccess)
                return NotFound(result); // Record not found or update failed

            _cache.Remove("all_employees_data");

            return Ok(result); // Noocntent ki bajaye Result bhej rahe hain taake message mil sake
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            _cache.Remove("all_employees_data");

            return Ok(result); // Result object with success message
        }
    }
}