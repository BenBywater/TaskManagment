using TaskManagement.TaskService.DTOs;

namespace TaskManagement.TaskService.Clients;

public interface INotificationClient
{
    Task NotifyTaskAssignedAsync(TaskResponse task);
}
