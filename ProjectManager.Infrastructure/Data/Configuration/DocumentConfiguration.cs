using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Entities.Models;

namespace ProjectManager.Infrastructure.Data.Configuration;

public class DocumentConfiguration : IEntityTypeConfiguration<Entities.Models.Document>
{
    public void Configure(EntityTypeBuilder<Entities.Models.Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title)
            .HasMaxLength(63)
            .IsRequired();

        builder.Property(d => d.FilePath)
            .HasMaxLength(127)
            .IsRequired();

        builder.HasOne(d => d.Project)
            .WithMany(p => p.Documents)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}