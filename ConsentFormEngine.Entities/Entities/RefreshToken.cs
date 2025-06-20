﻿using ConsentFormEngine.Core.Base;

namespace ConsentFormEngine.Entities.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }

        public string? ReasonRevoked { get; set; }

        public virtual User User { get; set; } = null!;

        public RefreshToken()
        {
            Token = string.Empty;
            CreatedByIp = string.Empty;
        }

        public RefreshToken(Guid userId, string token, DateTime expires, string createdByIp)
        {
            UserId = userId;
            Token = token;
            Expires = expires;
            CreatedByIp = createdByIp;
        }

        public RefreshToken(Guid id, Guid userId, string token, DateTime expires, string createdByIp)
        {
            UserId = userId;
            Token = token;
            Expires = expires;
            CreatedByIp = createdByIp;
        }
    }
}
