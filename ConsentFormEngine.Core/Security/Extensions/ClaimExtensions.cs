using System.Security.Claims;

namespace ConsentFormEngine.Core.Security.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddEmail(this ICollection<Claim> claims, string email) =>
       claims.Add(new Claim("email", email));

        public static void AddName(this ICollection<Claim> claims, string name) => claims.Add(new Claim("name", name));

        public static void AddNameIdentifier(this ICollection<Claim> claims, string nameIdentifier) =>
            claims.Add(new Claim("userId", nameIdentifier));

        public static void AddUserState(this ICollection<Claim> claims, string userState)
        {
            claims.Add(new Claim("userState", userState));
        }
    }
}
