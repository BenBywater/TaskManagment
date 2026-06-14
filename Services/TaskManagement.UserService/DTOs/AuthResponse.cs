namespace TaskManagement.UserService.DTOs;

public class AuthResponse
{
    // JWT token
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    // Return a list as a User may have several roles
    public IList<string> Roles { get; set; } = new List<string>();
}