using EmployeeAPI.Dtos;

public interface IEmployeeRepository
{
    Task<IEnumerable<EmployeeReadDto>> GetAllAsync(string? name, string? department, int pageNumber, int pageSize);
    Task<EmployeeReadDto?> GetByIdAsync(int id);
    Task<EmployeeReadDto> AddAsync(EmployeeCreateDto dto);
    Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}