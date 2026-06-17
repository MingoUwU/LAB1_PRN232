using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN232.LMS.Repositories;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly LmsDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(LmsDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<TokenResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            return await GenerateTokens(user);
        }

        public async Task<TokenResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var tokenEntity = await _context.Set<RefreshToken>()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked);

            if (tokenEntity == null || tokenEntity.ExpiryDate <= DateTime.UtcNow)
            {
                return null; // Token is invalid or expired
            }

            // Revoke old token
            tokenEntity.IsRevoked = true;

            // Generate new tokens
            return await GenerateTokens(tokenEntity.User);
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var tokenEntity = await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (tokenEntity == null || tokenEntity.IsRevoked)
            {
                return false;
            }

            tokenEntity.IsRevoked = true;
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<TokenResponse> GenerateTokens(User user)
        {
            // Create Access Token
            var jwtSecret = _config["Jwt:Key"] ?? Environment.GetEnvironmentVariable("JWT_SECRET") ?? "SuperSecretKeyForDevelopmentOnly123456!";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Create Refresh Token
            var refreshTokenString = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshTokenString,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                UserId = user.UserId
            };

            _context.Set<RefreshToken>().Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                ExpiresIn = 15 * 60
            };
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
