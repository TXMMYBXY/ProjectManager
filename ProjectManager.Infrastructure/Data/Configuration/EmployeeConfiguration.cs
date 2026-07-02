using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Entities.Models;

namespace ProjectManager.Infrastructure.Data.Configuration;

public class EmployeeConfiguration : IEntityTypeConfiguration<Entities.Models.Employee>
{
    public void Configure(EntityTypeBuilder<Entities.Models.Employee> builder)
    {
        builder.ToTable("Employees");
        
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.FirstName)
            .HasMaxLength(50)
            .IsRequired();
        
        builder
            .Property(e => e.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(e => e.Patronymic)
            .HasMaxLength(50);

        builder
            .Property(p => p.Role)
            .HasColumnType("tinyint")
            .IsRequired();

        builder
            .HasIndex(e => e.Email)
            .IsUnique();
        
        builder
            .HasMany(e => e.ExecutedIssues)
            .WithOne(i => i.Executor)
            .HasForeignKey(i => i.ExecutorId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}