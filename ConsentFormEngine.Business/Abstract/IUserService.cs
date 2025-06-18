using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Core.Utilities;
using static ConsentFormEngine.Business.DTOs.LoginResponseDto;

namespace ConsentFormEngine.Business.Abstract
{
    public interface IUserService
    {
        Task<Result> ForgotOrCreateUserAsync(ForgotOrCreateUserRequestDto dto);
        Task<Result> CreateUserAsync(CreateUserRequestDto dto);
        Task<Result> SendForgotPasswordMail(string mail, string baseUrl);
        Task<DataResult<LoggedHttpResponse>> Login(UserForLoginDto userForLoginDto);
    }
}
