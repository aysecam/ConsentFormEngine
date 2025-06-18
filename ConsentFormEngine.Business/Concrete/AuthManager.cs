using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ConsentFormEngine.Business.Abstract;
using ConsentFormEngine.Business.Security.JWT;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.Entities.Entities;

namespace ConsentFormEngine.Business.Concrete
{
    public class AuthManager: IAuthService
    {
        private readonly ITokenHelper _tokenHelper;
        private readonly TokenOptions _tokenOptions;
        private readonly IRepository<User> _userService;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;

        public AuthManager(ITokenHelper tokenHelper, IConfiguration configuration, IRepository<User> userService, IRepository<RefreshToken> refreshTokenRepository)
        {
            _tokenHelper = tokenHelper;

            const string tokenOptionsConfigurationSection = "TokenOptions";
            _tokenOptions =
                configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
                ?? throw new NullReferenceException($"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration");
            _userService = userService;
            _refreshTokenRepository = refreshTokenRepository;
            _userService = userService;
        }

        public async Task<AccessToken> CreateAccessToken(User request)
        {
            var user = await _userService.GetAsync(
                             x => x.Id == request.Id
    
            );
            AccessToken accessToken = _tokenHelper.CreateToken(user!);
            return accessToken;
        }

        public Task<RefreshToken> CreateRefreshToken(User user, string ipAddress)
        {
            RefreshToken refreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);
            return Task.FromResult(refreshToken);
        }

        public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
        {
            RefreshToken addedRefreshToken = await _refreshTokenRepository.AddAsync(refreshToken);
            return addedRefreshToken;
        }
        public async Task DeleteOldRefreshTokens(Guid userId)
        {
            var expiredTokens = await _refreshTokenRepository
                .Query()
                .AsNoTracking()
                .Where(rt =>
                    rt.UserId == userId &&
                    rt.Revoked == null &&
                    rt.Expires < DateTime.UtcNow)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                await _refreshTokenRepository.DeleteRangeAsync(expiredTokens);
            }
        }

        public async Task RevokeRefreshToken(RefreshToken refreshToken, string ipAddress, string? reason = null, string? replacedByToken = null)
        {
            refreshToken.Revoked = DateTime.Now;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReasonRevoked = reason;
            refreshToken.ReplacedByToken = replacedByToken;
            await _refreshTokenRepository.Update(refreshToken);
        }
    }
}
