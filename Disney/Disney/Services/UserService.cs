using Disney.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Disney.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManger;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _mailService;

        public UserService(IConfiguration configuration, IEmailSender mailService, UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _mailService = mailService;
            _userManger = userManager;
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterModel model)
        {
            // Verificamos que email y password no esten vacios.
            if (String.IsNullOrEmpty(model.Email) || String.IsNullOrEmpty(model.Password))
                return new UserManagerResponse
                {
                    Message = "Datos inválidos",
                    IsSuccess = false,
                };

            // Se validan formatos correctos
            // Validamos que la contraseña cumpla con ciertos criterios
            if (!Regex.IsMatch(model.Password, "^(?=\\w*\\d)(?=\\w*[A-Z])(?=\\w*[a-z])\\S{8,16}$"))
                return new UserManagerResponse
                {
                    Message = "La contraseña debe tener al entre 8 y 16 caracteres, al menos un dígito, " +
                    "al menos una minúscula y al menos una mayúscula.",
                    IsSuccess = false,
                };
           
            // Validamos que el mail ingresado sea valido
            if (!Regex.IsMatch(model.Email, "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$"))
                return new UserManagerResponse
                {
                    Message = "El email ingresado no es válido",
                    IsSuccess = false,
                };

            // Verificamos que el password coincida
            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = "Las contraseñas no coinciden",
                    IsSuccess = false,
                };

            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.UserName,
            };

            var result = await _userManger.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                await _mailService.SendEmailAsync(identityUser.Email, "Bienvenido", "Su cuenta se ha creado con exito!!!");

                return new UserManagerResponse
                {
                    Message = "Usuario creado correctamente",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "El usuario no pudo ser creado",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }
        public async Task<UserManagerResponse> LoginUserAsync(LoginModel model)
        {
            // Validaciones de los campos recibidos
            if (string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.Password))
                return new UserManagerResponse
                {
                    Message = "No pueden estar ambos campos vacios!",
                    IsSuccess = false,
                };
            
            if (string.IsNullOrEmpty(model.Password))
                return new UserManagerResponse
                {
                    Message = "No puede estar el campo de la contraseña vacio!",
                    IsSuccess = false,
                };
            
            if (string.IsNullOrEmpty(model.Email))
                return new UserManagerResponse
                {
                    Message = "No pueden estar el campo del email vacio!",
                    IsSuccess = false,
                };

            // Validacion de los datos recibidos
            var user = await _userManger.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "El usuario no existe",
                    IsSuccess = false,
                };
            }

            var result = await _userManger.CheckPasswordAsync(user, model.Password);

            if (!result)
                return new UserManagerResponse
                {
                    Message = "Contraseña Incorrecta",
                    IsSuccess = false,
                };

            return new UserManagerResponse
            {
                Message = "Usuario Logueado!",
                IsSuccess = true,
            };


        }
    }
}
