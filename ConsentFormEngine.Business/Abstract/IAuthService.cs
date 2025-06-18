using ConsentFormEngine.Business.Security.JWT;
using ConsentFormEngine.Entities.Entities;

namespace ConsentFormEngine.Business.Abstract
{
    public interface IAuthService
    {
        public Task<AccessToken> CreateAccessToken(User user);
        public Task<RefreshToken> CreateRefreshToken(User user, string ipAddress);
        public Task RevokeRefreshToken(RefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null);
        Task<RefreshToken> AddRefreshToken(RefreshToken token);
        Task DeleteOldRefreshTokens(Guid userId);
    }
}
