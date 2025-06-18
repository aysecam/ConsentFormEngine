using Microsoft.IdentityModel.Tokens;

namespace ConsentFormEngine.Core.Security.Encryption;
public static class SigningCredentialsHelper
{
    public static SigningCredentials CreateSigningCredentials(SecurityKey securityKey) =>
        new(securityKey, SecurityAlgorithms.HmacSha512Signature);
}
