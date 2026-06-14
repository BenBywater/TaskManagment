using TaskManagement.UserService.DTOs;
using TaskManagement.UserService.Models;

namespace TaskManagement.UserService.Interfaces;

public interface ITokenService
{
        Task<AuthResponse> GenerateTokenAsync(User user, IList<string> roles);
}
