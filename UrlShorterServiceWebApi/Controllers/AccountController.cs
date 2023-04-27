using Microsoft.AspNetCore.Mvc;
using UrlShorterServiceWebApi.Interfaces;
using UrlShorterServiceWebApi.Models;

namespace UrlShorterServiceWebApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody] RegisterUserModel user)
        {
            try
            {
                await accountService.Register(user);
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Accepted();
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginUserModel user)
        {
            try
            {
                var token = await accountService.Login(user);
                var userDto = new UserDto()
                {
                    Token= token,
                };
                return Ok(userDto);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
