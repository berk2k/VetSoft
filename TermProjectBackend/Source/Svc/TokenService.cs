using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly int _accessTokenExpiryMinutes;
        private readonly int _refreshTokenExpiryDays;
        private readonly VetDbContext _context;

        public TokenService(IConfiguration configuration, VetDbContext context)
        {
            _secretKey = configuration["ApiSettings:Secret"];
            _context = context;

            _accessTokenExpiryMinutes = configuration.GetValue<int>("ApiSettings:AccessTokenExpiryMinutes", 60);
            _refreshTokenExpiryDays = configuration.GetValue<int>("ApiSettings:RefreshTokenExpiryDays", 7);
        }

        public string GenerateToken(int userId, string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            // Generate a cryptographically secure random token
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<RefreshTokenResponse> CreateRefreshTokenAsync(int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
                Created = DateTime.UtcNow,
                IsRevoked = false
            };

            // First invalidate any existing refresh tokens for this user (optional)
            // You might want to keep previous tokens valid in some cases
            var existingTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in existingTokens)
            {
                token.IsRevoked = true;
            }

            // Add the new token
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new RefreshTokenResponse
            {
                RefreshToken = refreshToken.Token,
                ExpiryDate = refreshToken.ExpiryDate
            };
        }

        public async Task<TokenRefreshResult> RefreshAccessTokenAsync(string refreshToken)
        {
            // Check if refresh token exists and is valid
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.AppUser)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (storedToken == null)
            {
                return new TokenRefreshResult { Success = false, Error = "Invalid refresh token" };
            }

            if (storedToken.ExpiryDate < DateTime.UtcNow)
            {
                // Token has expired, mark it as revoked
                storedToken.IsRevoked = true;
                await _context.SaveChangesAsync();
                return new TokenRefreshResult { Success = false, Error = "Refresh token expired" };
            }

            // Generate new access token
            var newAccessToken = GenerateToken(storedToken.UserId, storedToken.AppUser.UserName);

            // Optionally, you can rotate refresh tokens for better security
            // This is recommended as a security best practice
            var newRefreshTokenResponse = await CreateRefreshTokenAsync(storedToken.UserId);

            // Revoke the used refresh token
            storedToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            return new TokenRefreshResult
            {
                Success = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenResponse.RefreshToken,
                RefreshTokenExpiry = newRefreshTokenResponse.ExpiryDate
            };
        }

        public async Task RevokeRefreshTokenAsync(int userId, string refreshToken = null)
        {
            // If refresh token is provided, revoke only that token
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);
                if (token != null)
                {
                    token.IsRevoked = true;
                    await _context.SaveChangesAsync();
                }
                return;
            }

            // Otherwise revoke all refresh tokens for the user
            var userTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}

