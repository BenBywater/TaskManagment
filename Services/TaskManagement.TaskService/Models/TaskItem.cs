using System.ComponentModel.DataAnnotations;

namespace TaskManagement.TaskService.Models;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
    public DateTime? DueDate { get; set; }

    // Denormalized from JWT / ProjectService — no cross-service joins
    public Guid ProjectId { get; set; }
    public string AssigneeId { get; set; } = string.Empty;
    public string AssigneeName { get; set; } = string.Empty;
    public string CreatedById { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
