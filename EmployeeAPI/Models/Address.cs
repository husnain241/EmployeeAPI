namespace EmployeeAPI.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public string AddressType { get; set; } = string.Empty; // e.g. "Home" ya "Office"
        // Foreign Key
        public int EmployeeId { get; set; }
    }
}
