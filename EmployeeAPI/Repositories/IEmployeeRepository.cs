using EmployeeAPI.Common;
using EmployeeAPI.Dtos;

public interface IEmployeeRepository
{
    Task<Result<IEnumerable<EmployeeReadDto>>> GetAllAsync(string? name, string? department, int pageNumber, int pageSize);
    Task<Result<EmployeeReadDto>> GetByIdAsync(int id);
    Task<Result<EmployeeReadDto>> AddAsync(EmployeeCreateDto dto);
    Task<Result<bool>> UpdateAsync(int id, EmployeeUpdateDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}