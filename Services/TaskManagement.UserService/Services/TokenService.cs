using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.UserService.DTOs;
using TaskManagement.UserService.Interfaces;
using TaskManagement.UserService.Models;

namespace TaskManagement.UserService.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IConfiguration configuration)
    {
        // Deserialise JwSettings block in appsetting.json
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings section is missing from configuration.");
    }

    public async Task<AuthResponse> GenerateTokenAsync(User user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // After the token expires, the user will be required to log in again
        var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        // Create Role claims from list
        IList<Claim> roleClaims = new List<Claim>();
        foreach(var r in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, r));
        }

        // Key Value pairs included in the token payload
        // Concat the 
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }.Concat(roleClaims);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

        // WriteToken serialises the token object
        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiry,
            UserId = user.Id,
            Email = user.Email!,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = roles
        };
    }
}
