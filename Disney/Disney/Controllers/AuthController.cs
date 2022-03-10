using Disney.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Disney.Models.Auth;

namespace Disney.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private string generatedToken = null;

        public AuthController(IConfiguration config, ITokenService tokenService, IUserService userService)
        {
            _config = config;
            _tokenService = tokenService;
            _userService = userService;
        }

        // POST: api/Auth/register
        [HttpPost, Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel user)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(user);
                if (result.IsSuccess)
                    return Ok(result); // Status Code: 200 

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Auth/login
        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel user)
        {
            try
            {
                var result = await _userService.LoginUserAsync(user);

                if (result.IsSuccess)
                {
                    generatedToken = _tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(),
                    user);

                    if (generatedToken != null)
                    {
                        HttpContext.Session.SetString("Token", generatedToken);
                        return Ok(result);
                    }
                    else
                    {
                        return StatusCode(500, "Internal server error");
                    }
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    }
}
