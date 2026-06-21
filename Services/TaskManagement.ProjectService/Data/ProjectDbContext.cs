using Microsoft.EntityFrameworkCore;
using TaskManagement.ProjectService.Models;

namespace TaskManagement.ProjectService.Data;

public class ProjectDbContext : DbContext
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> Members => Set<ProjectMember>();

    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Project
        modelBuilder.Entity<Project>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(100).IsRequired();
            e.Property(p => p.Description).HasMaxLength(500);
            e.Property(p => p.OwnerId).HasMaxLength(450).IsRequired();
            e.Property(p => p.OwnerName).HasMaxLength(200).IsRequired();
        });

        // ProjectMember
        modelBuilder.Entity<ProjectMember>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.UserId).HasMaxLength(450).IsRequired();
            e.Property(m => m.UserName).HasMaxLength(200).IsRequired();

            // Define one to many relationship with Project and Project Members
            e.HasOne(m => m.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(m => m.ProjectId)
            // If project is deleted also delete any associated project members
            .OnDelete(DeleteBehavior.Cascade);

            // Prevent the same user being added to a project twice
            e.HasIndex(m => new { m.ProjectId, m.UserId }).IsUnique();
        });
    }
}