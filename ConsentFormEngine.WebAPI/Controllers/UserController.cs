using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ConsentFormEngine.Business.Abstract;
using ConsentFormEngine.Business.DTOs;

namespace ConsentFormEngine.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("forgot-or-create")]
        public async Task<IActionResult> ForgotOrCreate([FromBody] ForgotOrCreateUserRequestDto dto)
        {
            var result = await _userService.ForgotOrCreateUserAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("send-forgot-password-mail")]
        public async Task<IActionResult> SendMail([FromQuery] string mail)
        {
            string baseUrl = Request.Headers.First(f => f.Key == "Referer").Value.ToString().TrimEnd('/');

            var result = await _userService.SendForgotPasswordMail(mail,baseUrl);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
