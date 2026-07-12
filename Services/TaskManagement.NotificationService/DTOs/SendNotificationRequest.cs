using System.ComponentModel.DataAnnotations;
using TaskManagement.NotificationService.Models;

namespace TaskManagement.NotificationService.DTOs;

public class SendNotificationRequest
{
    [Required]
    public NotificationType Type { get; set; }

    [Required]
    public string RecipientId { get; set; } = string.Empty;

    [Required]
    public string RecipientName { get; set; } = string.Empty;

    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public string TaskTitle { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;
}
