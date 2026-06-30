using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Entities.Models;

namespace ProjectManager.Infrastructure.Data.Configuration;

public class EmployeeProjectConfiguration : IEntityTypeConfiguration<EmployeeProject>
{
    public void Configure(EntityTypeBuilder<EmployeeProject> builder)
    {
        builder.ToTable("EmployeeJoinProject");

        builder.HasKey(j => j.Id);
        
        builder
            .HasOne(j => j.Employee)
            .WithMany(e => e.EmployeeProjects)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(j => j.Project)
            .WithMany(p => p.EmployeeProjects)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}