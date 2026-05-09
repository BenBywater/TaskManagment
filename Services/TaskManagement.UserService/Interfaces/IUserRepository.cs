using TaskManagement.UserService.Models;

namespace TaskManagement.UserService.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
    Task<User> CreateAsync(User user, string password);
    Task UpdateAsync(User user);
    Task<bool> CheckPasswordAsync(User user, string password);
}

