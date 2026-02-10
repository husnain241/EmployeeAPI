using EmployeeAPI.Common;
using EmployeeAPI.Dtos;
using EmployeeAPI.Repositories;
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
        public EmployeesController(IEmployeeRepository repository,IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        [HttpGet]
       // [ResponseCache(Duration = 30)] // 60 seconds tak cache rahega
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string? name,
            [FromQuery] string? department,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            string cacheKey = "all_employees_data";

            if (!_cache.TryGetValue(cacheKey, out Result<IEnumerable<EmployeeReadDto>> cachedEmployees))
            {
                // 2. Agar nahi hai, toh Database se mangwayein
                cachedEmployees = await _repository.GetAllAsync(name, department, pageNumber, pageSize);
                //var result = await _repository.GetAllAsync(name, department, pageNumber, pageSize);

                // 3. Cache settings define karein
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)) // 10 min baad lazmi expire hoga
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2))  // Agar 2 min tak koi na poochay toh expire
                    .SetPriority(CacheItemPriority.Normal);

                // 4. Data ko cache mein save kar dein
                _cache.Set(cacheKey, cachedEmployees, cacheOptions);


            }
                // Get mein aksar hum direct result bhej dete hain kyunki ye fail kam hota hai
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
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _repository.AddAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result); // e.g., Email already exists

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