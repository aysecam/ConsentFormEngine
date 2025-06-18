
using ConsentFormEngine.Entities.Entities;

namespace ConsentFormEngine.Business.Security.JWT;
public interface ITokenHelper
{
    AccessToken CreateToken(User user);

    RefreshToken CreateRefreshToken(User user, string ipAddress);
}
