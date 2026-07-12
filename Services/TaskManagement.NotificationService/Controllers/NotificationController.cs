using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.NotificationService.DTOs;
using TaskManagement.NotificationService.Models;

namespace TaskManagement.NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly Channel<NotificationMessage> _channel;

    public NotificationController(Channel<NotificationMessage> channel)
    {
        _channel = channel;
    }

    [HttpPost]
    [EndpointSummary("Queue a notification for dispatch")]
    public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
    {
        var message = new NotificationMessage
        {
            Type          = request.Type,
            RecipientId   = request.RecipientId,
            RecipientName = request.RecipientName,
            TaskId        = request.TaskId,
            TaskTitle     = request.TaskTitle,
            Message       = request.Message
        };

        await _channel.Writer.WriteAsync(message);
        return Accepted(); // 202 — message is queued, not yet dispatched
    }
}
