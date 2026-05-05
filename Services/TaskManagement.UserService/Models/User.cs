using Microsoft.AspNetCore.Identity;

namespace TaskManagment.UserService.Models;

public class User : IdentityUser
{
    // No need to define a unique identifier here, as IdentityUser already provides one
    // along with several other built-in properties.

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}