using System.ComponentModel.DataAnnotations;

namespace TaskManagement.TaskService.DTOs;

public class CreateTaskRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public Guid ProjectId { get; set; }

    public DateTime? DueDate { get; set; }
    public string AssigneeId { get; set; } = string.Empty;
    public string AssigneeName { get; set; } = string.Empty;
}
