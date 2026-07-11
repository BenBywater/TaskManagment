using System.ComponentModel.DataAnnotations;
using TaskManagement.TaskService.Models;
using TaskStatus = TaskManagement.TaskService.Models.TaskStatus;

namespace TaskManagement.TaskService.DTOs;

public class UpdateTaskStatusRequest
{
    [Required]
    public TaskStatus Status { get; set; }

    [Required]
    public byte[] RowVersion { get; set; } = [];
}
