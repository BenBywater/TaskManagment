namespace TaskManagement.UserSerive.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty; // JWT token
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}