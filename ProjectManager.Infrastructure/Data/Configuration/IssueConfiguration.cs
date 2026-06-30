using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Entities.Models;

namespace ProjectManager.Infrastructure.Data.Configuration;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.ToTable("Issues");
        
        builder.HasKey(i => i.Id);

        builder
            .Property(i => i.Title)
            .HasMaxLength(50)
            .IsRequired();
        
        builder
            .Property(i => i.Status)
            .HasColumnType("tinyint");

        builder
            .Property(i => i.Comments)
            .HasColumnType("nvarchar(max)");
        
        builder
            .Property(i => i.Priority)
            .HasColumnType("tinyint");

        builder
            .HasOne(i => i.Project)
            .WithMany(p => p.Issues)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(i => i.Author)
            .WithMany(e => e.AuthoredIssues)
            .HasForeignKey(i => i.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(i => i.Executor)
            .WithMany(e => e.ExecutedIssues)
            .HasForeignKey(i => i.ExecutorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}