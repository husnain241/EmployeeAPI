using EmployeeAPI.Data;
using EmployeeAPI.Dtos;
using EmployeeAPI.Models;
using EmployeeAPI.Common; // Result class ka namespace
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;
        public EmployeeRepository(AppDbContext context) => _context = context;

        public async Task<Result<IEnumerable<EmployeeReadDto>>> GetAllAsync(string? name, string? department, int pageNumber, int pageSize)
        {
            IQueryable<Employee> query = _context.Employees.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(e => e.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(e => e.Department == department);

            var data = await query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmployeeReadDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Department = e.Department,
                    Age = e.Age,
                    Email = e.Email,
                    Detail=e.Detail,
                    Cities = e.Addresses.Select(a => a.City).ToList()

                }).ToListAsync();

            return Result<IEnumerable<EmployeeReadDto>>.Success(data, "Employees fetched successfully");
        }

        public async Task<Result<EmployeeReadDto>> GetByIdAsync(int id)
        {
            var employee = await _context.Employees
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

            if (employee == null)
                return Result<EmployeeReadDto>.Failure($"Employee with ID {id} not found");

            return Result<EmployeeReadDto>.Success(employee);
        }

        public async Task<Result<EmployeeReadDto>> AddAsync(EmployeeCreateDto dto)
        {
            // Optional: Business logic check
            var exists = await _context.Employees.AnyAsync(e => e.Email == dto.Email);
            if (exists) return Result<EmployeeReadDto>.Failure("Email already exists");

            var employee = new Employee
            {
                Name = dto.Name,
                Department = dto.Department,
                Age = dto.Age,
                Email = dto.Email
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            var readDto = new EmployeeReadDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Department = employee.Department,
                Age = employee.Age,
                Email = employee.Email
            };

            return Result<EmployeeReadDto>.Success(readDto, "Employee created successfully");
        }

        public async Task<Result<bool>> UpdateAsync(int id, EmployeeUpdateDto dto)
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
                await _context.SaveChangesAsync();
                return Result<bool>.Success(true, "Employee updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<bool>.Failure("Update failed: Employee not found");
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return Result<bool>.Failure("Delete failed: Employee not found");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true, "Employee deleted successfully");
        }
    }
}