using System.Threading.Channels;
using TaskManagement.NotificationService.Models;

namespace TaskManagement.NotificationService.Services;

public class NotificationDispatcherService : BackgroundService
{
    private readonly Channel<NotificationMessage> _channel;
    private readonly ILogger<NotificationDispatcherService> _logger;

    public NotificationDispatcherService(
        Channel<NotificationMessage> channel,
        ILogger<NotificationDispatcherService> logger)
    {
        _channel = channel;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ReadAllAsync blocks until a message arrives, then yields it.
        // The loop runs for the lifetime of the app.
        await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            // Dispatch to all handlers in parallel
            // SimulateEmailAsync and SimulatePushAsync run in parallel and WhenAll will only finish when both are done
            await Task.WhenAll(
                SimulateEmailAsync(message),
                SimulatePushAsync(message)
            );
        }
    }

    // Fake these calls as we do not have an email or push services
    private Task SimulateEmailAsync(NotificationMessage message)
    {
        _logger.LogInformation(
            "[EMAIL] → {Recipient}: {Message}",
            message.RecipientName, message.Message);
        return Task.CompletedTask;
    }

    private Task SimulatePushAsync(NotificationMessage message)
    {
        _logger.LogInformation(
            "[PUSH] → {Recipient}: {Message}",
            message.RecipientName, message.Message);
        return Task.CompletedTask;
    }
}