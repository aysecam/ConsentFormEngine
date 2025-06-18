using Microsoft.AspNetCore.Mvc;
using ConsentFormEngine.Business.Abstract;
using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Entities.Entities;
using System.Linq;
using static ConsentFormEngine.Business.DTOs.LoginResponseDto;

namespace ConsentFormEngine.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto dto)
        {
            var result = await _userService.Login(dto);
            if (!result.Success)
            {
                return BadRequest(result);

            }
            if (result.Data.RefreshToken is not null)
                setRefreshTokenToCookie(result.Data.RefreshToken);

            var data = result.Data;
            var response = new LoggedHttpResponse
            {
                AccessToken = data.AccessToken,
                Expiration = data.Expiration,
                RefreshToken = data.RefreshToken!
            };

            if (result.Success)
                return Ok(response);

            return Unauthorized(result);
        }

        private void setRefreshTokenToCookie(string refreshToken)
        {
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Secure = true, // Only send cookie over HTTPS
                Expires = DateTime.Now.AddDays(7) //refreshToken.Expires
            };
            Response.Cookies.Append(key: "refreshToken", refreshToken, cookieOptions);
        }
    }
}
