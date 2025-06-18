using ConsentFormEngine.Business.Security.JWT;
using ConsentFormEngine.Entities.Entities;

namespace ConsentFormEngine.Business.DTOs
{
    public class LoginResponseDto 
    {
        public AccessToken? AccessToken { get; set; }
        public RefreshToken? RefreshToken { get; set; }


        public LoggedHttpResponse ToHttpResponse() =>
            new()
            {
                AccessToken = AccessToken!.Token,
                RefreshToken = RefreshToken?.Token!,
                Expiration = AccessToken.Expiration
            };



        public class LoggedHttpResponse
        {
            public string AccessToken { get; set; } = null!;
            public DateTime Expiration { get; set; }
            public string RefreshToken { get; set; } = null!;
        }
    }
}
