using Microsoft.EntityFrameworkCore;

namespace TaskManagement.TaskService.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }
}
