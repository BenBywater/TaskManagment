using System.Net.Http.Json;
using TaskManagement.TaskService.DTOs;

namespace TaskManagement.TaskService.Clients;

public class NotificationClient : INotificationClient
{
    private readonly HttpClient _http;

    public NotificationClient(HttpClient http) => _http = http;

    public async Task NotifyTaskAssignedAsync(TaskResponse task)
    {
        var payload = new
        {
            // Assign type as int as we do not have the enum declaration
            type          = 0,
            recipientId   = task.AssigneeId,
            recipientName = task.AssigneeName,
            taskId        = task.Id,
            taskTitle     = task.Title,
            message       = $"You have been assigned to '{task.Title}'"
        };

        try
        {
            await _http.PostAsJsonAsync("/api/notifications", payload);
        }
        catch (Exception)
        {
            // Notification failure is non-fatal because the task was already saved
        }
    }
}
