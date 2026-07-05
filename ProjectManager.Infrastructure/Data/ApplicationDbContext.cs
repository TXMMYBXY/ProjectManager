using Microsoft.EntityFrameworkCore;
using ProjectManager.Entities.Models;
using ProjectManager.Infrastructure.Data.Configuration;

namespace ProjectManager.Infrastructure.Data;

public class ApplicationDbContext : DbContext
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
        
        base.OnModelCreating(modelBuilder);
    }
}
