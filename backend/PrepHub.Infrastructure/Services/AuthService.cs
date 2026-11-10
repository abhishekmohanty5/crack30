using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PrepHub.Core.DTOs;
using PrepHub.Core.Entities;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PrepHubDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        PrepHubDbContext context,
        IConfiguration config)
    {
        _userManager = userManager;
        _context = context;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            Batch = request.Batch,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Registration failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "Student");

        return await GenerateAuthResponseAsync(user, "Student");
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Student";

        return await GenerateAuthResponseAsync(user, role);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string token)
    {
        var existingToken = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token && t.RevokedAt == null);

        if (existingToken == null || existingToken.ExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var roles = await _userManager.GetRolesAsync(existingToken.User);
        var role = roles.FirstOrDefault() ?? "Student";

        // Revoke the current refresh token
        existingToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GenerateAuthResponseAsync(existingToken.User, role);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<UserProfileDto> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Student";

        return new UserProfileDto(
            user.Id,
            user.Name,
            user.Email ?? string.Empty,
            user.Batch,
            user.TargetCompany,
            user.InterviewDate,
            user.CreatedAt,
            role);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (!string.IsNullOrWhiteSpace(request.Name)) user.Name = request.Name;
        if (request.Batch != null) user.Batch = request.Batch;
        if (request.TargetCompany != null) user.TargetCompany = request.TargetCompany;
        if (request.InterviewDate.HasValue)
            user.InterviewDate = request.InterviewDate;

        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Student";

        return new UserProfileDto(
            user.Id,
            user.Name,
            user.Email ?? string.Empty,
            user.Batch,
            user.TargetCompany,
            user.InterviewDate,
            user.CreatedAt,
            role);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(ApplicationUser user, string role)
    {
        var jwtKey = _config["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
        var issuer = _config["Jwt:Issuer"] ?? "PrepHub";
        var audience = _config["Jwt:Audience"] ?? "PrepHub";
        var expirationMinutes = int.TryParse(_config["Jwt:AccessTokenExpirationMinutes"], out var exp) ? exp : 60;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        // Generate Refresh Token
        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshExpDays = int.TryParse(_config["Jwt:RefreshTokenExpirationDays"], out var rExp) ? rExp : 7;

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshExpDays)
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        var userDto = new UserDto(user.Id, user.Name, user.Email ?? string.Empty, role);
        return new AuthResponse(accessToken, refreshTokenValue, userDto);
    }
}
