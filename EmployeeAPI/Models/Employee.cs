using System.ComponentModel.DataAnnotations;




namespace EmployeeAPI.Models
{

    public class Employee
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Range(18, 65)]
        public int Age { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required, StringLength(500)]
        public string Detail { get; set; } = string.Empty;

    }
}
