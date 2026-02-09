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


        ////NORMAL METHOD

        //public async Task<IEnumerable<Employee>> GetAllAsync(string? name, string? department)
        //{
        //    IQueryable<Employee> query = _context.Employees;

        //    if (!string.IsNullOrWhiteSpace(name))
        //        query = query.Where(e => e.Name.Contains(name));

        //    if (!string.IsNullOrWhiteSpace(department))
        //        query = query.Where(e => e.Department == department);

        //    return await query.ToListAsync();
        //}


        /// <summary>
        /// Optimazation Method
        /// 
        /// 
        public async Task<IEnumerable<EmployeeReadDto>> GetAllAsync(
     string? name,
     string? department,
     int pageNumber = 1,
     int pageSize = 10) // Optional params hamesha end mein
        {
            IQueryable<Employee> query = _context.Employees.AsNoTracking();

            // Filtering
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(e => e.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(e => e.Department == department);

            // Pagination logic
            return await query
                .OrderBy(e => e.Id) // Pagination ke liye OrderBy lazmi hai
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmployeeReadDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Department = e.Department,
                    Age = e.Age,
                    Email = e.Email
                })
                .ToListAsync();
        }

        /// </summary>


        public Task<Employee?> GetByIdAsync(int id) =>
            _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

        public async Task AddAsync(Employee employee) =>
            await _context.Employees.AddAsync(employee);

        public void Update(Employee employee) =>
            _context.Employees.Update(employee);

        public void Remove(Employee employee) =>
            _context.Employees.Remove(employee);

        public async Task<bool> SaveChangesAsync() =>
            await _context.SaveChangesAsync() > 0;


       
    }

}
