using Microsoft.EntityFrameworkCore;
using TaskManagement.TaskService.Data;
using TaskManagement.TaskService.Interfaces;
using TaskManagement.TaskService.Models;

namespace TaskManagement.TaskService.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _db;

    public TaskRepository(TaskDbContext db)
    {
        _db = db;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskItem>> GetByProjectAsync(Guid projectId)
    {
        return await _db.Tasks
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByAssigneeAsync(string assigneeId)
    {
        return await _db.Tasks
            .Where(t => t.AssigneeId == assigneeId)
            .ToListAsync();
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _db.Tasks.Update(task);
        await _db.SaveChangesAsync(); // throws DbUpdateConcurrencyException on RowVersion mismatch
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
    }
}
