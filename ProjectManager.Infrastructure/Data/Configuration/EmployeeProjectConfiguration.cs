using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Entities.Models;

namespace ProjectManager.Infrastructure.Data.Configuration;

public class EmployeeProjectConfiguration : IEntityTypeConfiguration<EmployeeProject>
{
    public void Configure(EntityTypeBuilder<EmployeeProject> builder)
    {
        builder.ToTable("EmployeeJoinProject");

        builder.HasKey(x => new { x.EmployeeId, x.ProjectId });
        
        builder
            .HasOne(ep => ep.Employee)
            .WithMany(e => e.EmployeeProjects)
            .HasForeignKey(ep => ep.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(ep => ep.Project)
            .WithMany(p => p.EmployeeProjects)
            .HasForeignKey(ep => ep.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}