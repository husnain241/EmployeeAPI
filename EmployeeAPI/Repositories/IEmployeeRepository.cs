using EmployeeAPI.Dtos;
using EmployeeAPI.Models;

namespace EmployeeAPI.Repositories
{
    // Repositories/IEmployeeRepository.cs
    public interface IEmployeeRepository
    {
        //Task<IEnumerable<Employee>> GetAllAsync(string? name, string? department);
        Task<IEnumerable<EmployeeReadDto>> GetAllAsync(string? name, string? department, int pageNumber, int pageSize);

        Task<Employee?> GetByIdAsync(int id);
        Task AddAsync(Employee employee);
        void Update(Employee employee);
        void Remove(Employee employee);
        Task<bool> SaveChangesAsync();

    }

}
