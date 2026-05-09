using Microsoft.AspNetCore.Identity;
using TaskManagement.UserService.Interfaces;
using TaskManagement.UserService.Models;

namespace TaskManagement.UserService.Repositories;

public class UserRepository : IUsereRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
    public async Task<bool> ExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
    public async Task<User> CreateAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        return user;
    }
    public async Task UpdateAsync(User user)
    {
        await _userManager.UpdateAsync(user);
    }
    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }
}