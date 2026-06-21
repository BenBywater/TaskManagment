using System.ComponentModel.DataAnnotations;

namespace TaskManagement.ProjectService.DTOs;
public class AddMemberRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;
}