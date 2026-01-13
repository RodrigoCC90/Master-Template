using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrbitOS.Application.Interfaces;
using OrbitOS.Domain.Entities;
using OrbitOS.Infrastructure.Data;

namespace OrbitOS.Api.Controllers;

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginResponse
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class GoogleLoginRequest
{
    public required string Credential { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthController> _logger;
    private readonly OrbitOSDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthController(
        ICurrentUserService currentUserService,
        ILogger<AuthController> logger,
        OrbitOSDbContext dbContext,
        IConfiguration configuration)
    {
        _currentUserService = currentUserService;
        _logger = logger;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return Unauthorized(new { Message = "Invalid email or password" });
        }

        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { Message = "Invalid email or password" });
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        var token = GenerateJwtToken(user.Id.ToString(), user.Email, user.DisplayName);
        var expiresAt = DateTime.UtcNow.AddDays(7);

        return Ok(new LoginResponse
        {
            Token = token,
            Email = user.Email,
            DisplayName = user.DisplayName,
            ExpiresAt = expiresAt
        });
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            // Verify the Google token
            var googleUser = await VerifyGoogleToken(request.Credential);
            if (googleUser == null)
            {
                return Unauthorized(new { Message = "Invalid Google token" });
            }

            // Find or create user
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.GoogleId == googleUser.Sub || u.Email.ToLower() == googleUser.Email.ToLower());

            if (user == null)
            {
                // Create new user
                user = new User
                {
                    GoogleId = googleUser.Sub,
                    Email = googleUser.Email,
                    DisplayName = googleUser.Name,
                    FirstName = googleUser.GivenName,
                    LastName = googleUser.FamilyName,
                    AvatarUrl = googleUser.Picture
                };
                _dbContext.Users.Add(user);
            }
            else if (string.IsNullOrEmpty(user.GoogleId))
            {
                // Link Google account to existing user
                user.GoogleId = googleUser.Sub;
                if (string.IsNullOrEmpty(user.AvatarUrl))
                {
                    user.AvatarUrl = googleUser.Picture;
                }
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            var token = GenerateJwtToken(user.Id.ToString(), user.Email, user.DisplayName);
            var expiresAt = DateTime.UtcNow.AddDays(7);

            return Ok(new LoginResponse
            {
                Token = token,
                Email = user.Email,
                DisplayName = user.DisplayName,
                ExpiresAt = expiresAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google login failed");
            return Unauthorized(new { Message = "Google authentication failed" });
        }
    }

    private async Task<GoogleUserInfo?> VerifyGoogleToken(string credential)
    {
        var googleClientId = _configuration["Google:ClientId"]
            ?? "794543200576-ugbq975aqa7s4ie74t7rvqkaskj489hd.apps.googleusercontent.com";

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={credential}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var tokenInfo = JsonSerializer.Deserialize<GoogleUserInfo>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Verify the token is for our app
        if (tokenInfo?.Aud != googleClientId)
        {
            return null;
        }

        return tokenInfo;
    }

    private class GoogleUserInfo
    {
        public string Sub { get; set; } = "";
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string Picture { get; set; } = "";
        public string Aud { get; set; } = "";
        public string GivenName { get; set; } = "";
        public string FamilyName { get; set; } = "";
    }

    private string GenerateJwtToken(string userId, string email, string displayName)
    {
        var key = _configuration["Jwt:Key"] ?? "OrbitOS-Development-Secret-Key-Min-32-Chars!!";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("name", displayName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "OrbitOS",
            audience: _configuration["Jwt:Audience"] ?? "OrbitOS",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        var hashBytes = Convert.FromBase64String(passwordHash);
        var salt = hashBytes[..16];
        var storedHash = hashBytes[16..];

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var computedHash = pbkdf2.GetBytes(32);

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        if (!_currentUserService.IsAuthenticated)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            ObjectId = _currentUserService.AzureAdObjectId,
            Email = _currentUserService.Email,
            DisplayName = _currentUserService.DisplayName,
            TenantId = _currentUserService.TenantId
        });
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { Message = "OrbitOS API is running", Timestamp = DateTime.UtcNow });
    }

    [HttpGet("ping-auth")]
    [Authorize]
    public IActionResult PingAuthenticated()
    {
        return Ok(new
        {
            Message = "Authenticated successfully",
            User = _currentUserService.Email,
            Timestamp = DateTime.UtcNow
        });
    }
}
