namespace TaskManagement.ProjectService.Models;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Denormalised from UserService JWT claims
    public string OwnerId { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
}
