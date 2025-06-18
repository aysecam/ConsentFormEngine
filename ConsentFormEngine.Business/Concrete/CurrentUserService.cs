using Microsoft.AspNetCore.Http;
using ConsentFormEngine.Core.Services;
using System.Security.Claims;

namespace ConsentFormEngine.Business.Concrete
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    return null; // veya "SYSTEM"

                // Eğer JWT ile kimlik doğrulama yapıldıysa ClaimTypes.NameIdentifier içindeki ID'yi al
                var claim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                    return null; // veya "SYSTEM"

                var guidId = Guid.Parse(claim.Value);

                return guidId;
            }
        }

        public string IpAddress
        {
            get
            {
                var context = _httpContextAccessor.HttpContext
                              ?? throw new InvalidOperationException("HTTP context mevcut değil.");

                // Eğer proxy arkasındaysa X-Forwarded-For header’ını kullan
                if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
                {
                    return forwarded.ToString()
                                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .First()
                                    .Trim();
                }

                // Aksi takdirde RemoteIpAddress’i al
                var ip = context.Connection.RemoteIpAddress
                         ?.MapToIPv4()
                         .ToString();

                return ip ?? throw new InvalidOperationException("Client IP adresi alınamadı.");
            }
        }
    }
}
