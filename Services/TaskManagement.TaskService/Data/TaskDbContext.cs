using Microsoft.EntityFrameworkCore;
using TaskManagement.TaskService.Models;

namespace TaskManagement.TaskService.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Title).HasMaxLength(200).IsRequired();
            e.Property(t => t.Description).HasMaxLength(2000);
            e.Property(t => t.Status).IsRequired();
            e.Property(t => t.AssigneeId).HasMaxLength(450);
            e.Property(t => t.AssigneeName).HasMaxLength(200);
            e.Property(t => t.CreatedById).HasMaxLength(450).IsRequired();
            e.Property(t => t.CreatedByName).HasMaxLength(200).IsRequired();
            e.Property(t => t.RowVersion).IsRowVersion();
        });
    }
}
