namespace TaskManagement.ProjectService.Models;

public class ProjectMember
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }

    // Navigation property
    // One to many relationship between Project and Project Members
    public Project Project { get; set; } = null!;

    // Denormalized from UserService JWT claims
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
