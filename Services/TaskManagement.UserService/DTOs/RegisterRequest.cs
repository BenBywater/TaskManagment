using System.ComponentModel.DataAnnotations;

namespace TaskManagement.UserSerive.DTOs;

public class RegisterRequest
{
    
    [Required]
    public string FirstName {get; set;} = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password {get; set; } = string.Empty;

    [Required]
    [Compare("Password", ErrorMessage ="Passwords do not match.")]
    public string ConfirmPassword { get; set;  } = string.Empty;
}