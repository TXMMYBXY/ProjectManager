using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Entities.Models;

namespace ProjectManager.Infrastructure.Data.Configuration;

public class ProjectConfiguration : IEntityTypeConfiguration<Entities.Models.Project>
{
    public void Configure(EntityTypeBuilder<Entities.Models.Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder
            .Property(p => p.Title)
            .HasMaxLength(50)
            .IsRequired();
        
        builder
            .Property(p => p.CompanyCustomer)
            .HasMaxLength(100)
            .IsRequired();
        
        builder
            .Property(p => p.CompanyExecuter)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(p => p.StartDate)
            .HasColumnType("date");
        
        builder
            .Property(p => p.FinishDate)
            .HasColumnType("date");
        
        builder
            .Property(p => p.Priority)
            .HasColumnType("tinyint");
        
        builder
            .HasOne(p => p.ProjectManager)
            .WithMany(e => e.ManagedProjects)
            .HasForeignKey(x => x.ProjectManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}