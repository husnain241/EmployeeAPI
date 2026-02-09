using EmployeeAPI.Data;
using EmployeeAPI.Dtos;
using EmployeeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Repositories
{

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;
        public EmployeeRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<EmployeeReadDto>> GetAllAsync(string? name, string? department, int pageNumber, int pageSize)
        {
            IQueryable<Employee> query = _context.Employees.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(e => e.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(e => e.Department == department);

            return await query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmployeeReadDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Department = e.Department,
                    Age = e.Age,
                    Email = e.Email
                }).ToListAsync();
        }

        public async Task<EmployeeReadDto?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EmployeeReadDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Department = e.Department,
                    Age = e.Age,
                    Email = e.Email
                }).FirstOrDefaultAsync();
        }

        public async Task<EmployeeReadDto> AddAsync(EmployeeCreateDto dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Department = dto.Department,
                Age = dto.Age,
                Email = dto.Email
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return new EmployeeReadDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Department = employee.Department,
                Age = employee.Age,
                Email = employee.Email
            };
        }

        public async Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto)
        {
            var employee = new Employee
            {
                Id = id,
                Name = dto.Name,
                Department = dto.Department,
                Age = dto.Age,
                Email = dto.Email
            };

            _context.Employees.Attach(employee);
            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Direct Delete Optimization
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            return await _context.SaveChangesAsync() > 0;
        }
    }

}
