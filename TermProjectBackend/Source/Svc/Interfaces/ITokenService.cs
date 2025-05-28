using TermProjectBackend.Models.Dto;
using TermProjectBackend.Models;

namespace TermProjectBackend.Source.Svc.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(int userId, string userName);

        string GenerateRefreshToken();


        Task<RefreshTokenResponse> CreateRefreshTokenAsync(int userId);


        Task<TokenRefreshResult> RefreshAccessTokenAsync(string refreshToken);


        Task RevokeRefreshTokenAsync(int userId, string refreshToken = null);
    }
}
