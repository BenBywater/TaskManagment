using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.UserService.DTOs;
using TaskManagement.UserService.Interfaces;
using TaskManagement.UserService.Models;

namespace TaskManagement.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthController(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _userRepository.ExistsAsync(request.Email))
        {
            return Conflict(new { message = "A user with this email already exists." });
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        await _userRepository.CreateAsync(user, request.Password);
        await _userRepository.AssignRoleAsync(user, "Member");
        IList<string> roles = await _userRepository.GetRolesAsync(user);

        return CreatedAtAction(nameof(Register), await _tokenService.GenerateTokenAsync(user, roles));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (!await _userRepository.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });   
        }

        user.LastLoginAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        IList<string> roles = await _userRepository.GetRolesAsync(user);

        return Ok(await _tokenService.GenerateTokenAsync(user, roles));
    }

[Authorize]
[HttpGet("me")]
public IActionResult Me()
{
    return Ok(new
    {
        userId   = User.FindFirstValue(JwtRegisteredClaimNames.Sub),
        email    = User.FindFirstValue(JwtRegisteredClaimNames.Email),
        fullName = $"{User.FindFirstValue(JwtRegisteredClaimNames.GivenName)} {User.FindFirstValue(JwtRegisteredClaimNames.FamilyName)}",
        roles    = User.FindAll(ClaimTypes.Role).Select(c => c.Value)
    });
}
}