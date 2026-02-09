using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Dtos
{
    public class EmployeeReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? Email { get; set; }

        public string Detail { get; set; } = string.Empty;
    }



public class EmployeeCreateDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Range(18, 65)]
        public int Age { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }

    public class EmployeeUpdateDto : EmployeeCreateDto { }

}
