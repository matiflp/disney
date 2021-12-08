using Disney.Contracts;
using Disney.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Disney.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
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

                User newUser = new User
                {
                    Email = user.Email,
                    Password = user.Password,
                    Name = user.Name
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
        public async Task<IActionResult> Login([FromBody] UserDTO user)
        {
            try
            {
                User userAuth = _userRepository.FindByEmail(user.Email);
                if (user == null || !String.Equals(userAuth.Password, user.Password))
                    return Unauthorized();

                var claims = new List<Claim>
                    {
                        new Claim("User", user.Email)
                    };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
