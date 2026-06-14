using Microsoft.EntityFrameworkCore;

namespace TaskManagement.ProjectService.Data;

public class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }
}