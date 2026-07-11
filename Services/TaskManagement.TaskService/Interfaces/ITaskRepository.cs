using TaskManagement.TaskService.Models;

namespace TaskManagement.TaskService.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskItem>> GetByProjectAsync(Guid projectId);
    Task<IEnumerable<TaskItem>> GetByAssigneeAsync(string assigneeId);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
}
