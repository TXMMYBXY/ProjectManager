using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Entities.Models;

namespace ProjectManager.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<Entities.Models.Employee, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
    {
        
    }
    
    public DbSet<Entities.Models.Employee> Employees { get; set; }
    public DbSet<Entities.Models.Project> Projects { get; set; }
    public DbSet<Entities.Models.Issue> Issues { get; set; }
    public DbSet<EmployeeProject> EmployeesProjects { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        var roles = new List<IdentityRole<int>>
        {
            new ()
            {
                Id = 1,
                Name = "Director",
                NormalizedName = "DIRECTOR"
            },
            new ()
            {
                Id = 2,
                Name = "Manager",
                NormalizedName = "MANAGER"
            },
            new ()
            {
                Id = 3,
                Name = "Employee",
                NormalizedName = "EMPLOYEE"
            }
        };
        
        modelBuilder.Entity<IdentityRole<int>>().HasData(roles);
        
        base.OnModelCreating(modelBuilder);
    }
}
