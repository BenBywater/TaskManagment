namespace TaskManagement.ProjectService.DTOs;
public class MemberResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}