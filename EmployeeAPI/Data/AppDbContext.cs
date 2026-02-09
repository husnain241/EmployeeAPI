using EmployeeAPI.Models;
using Microsoft.EntityFrameworkCore;




namespace EmployeeAPI.Data
{
    

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
    }

}
