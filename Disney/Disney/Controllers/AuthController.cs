using Disney.Auth;
using Disney.Models;
using Disney.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Disney.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private string generatedToken = null;

        public AuthController(IConfiguration config, ITokenService tokenService, IUserRepository userRepository)
        {
            _config = config;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO user)
        {
            try
            {
                // Verificamos que email y password no esten vacios.
                if (String.IsNullOrEmpty(user.Email) || String.IsNullOrEmpty(user.Password))
                    return StatusCode(403, "Datos inválidos");

                // Validamos que la contraseña cumpla con ciertos criterios
                if (!Regex.IsMatch(user.Password, "^(?=\\w*\\d)(?=\\w*[A-Z])(?=\\w*[a-z])\\S{8,16}$"))
                    return StatusCode(403, "La contraseña debe tener al entre 8 y 16 caracteres, al menos un dígito, " +
                        "al menos una minúscula y al menos una mayúscula.");

                // Validamos que el mail ingresado sea valido
                if (!Regex.IsMatch(user.Email, "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$"))
                    return StatusCode(403, "El email ingresado no es válido");

                // De ser válido obtenemos el mail y verificamos que no este en uso
                User dbUser = _userRepository.FindByEmail(user.Email);
                if (dbUser != null)
                    return StatusCode(403, "Email está en uso");

                User newUser = new()
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Password = user.Password,
                };

                _userRepository.Save(newUser);
                // Retornamos el nuevo jugador
                return StatusCode(201, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDTO user)
        {
            User validUser = _userRepository.GetUser(user);
            if (user == null || !String.Equals(validUser.Password, user.Password))
                return Unauthorized();

            //IActionResult response = Unauthorized();

            if (validUser != null)
            {
                generatedToken = _tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(),
                validUser);

                if (generatedToken != null)
                {
                    HttpContext.Session.SetString("Token", generatedToken);
                    return Ok();
                }
                else
                {
                    return StatusCode(500, "Internal server error");
                }
            }
            else
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
