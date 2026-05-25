using TaskManagement.UserService.DTOs;
using TaskManagement.UserService.Models;

namespace TaskManagement.UserService.Interfaces;

public interface ITokenService
{
    AuthResponse GenerateToken(User user);
}
