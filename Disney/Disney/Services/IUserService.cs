using Disney.Models.Auth;
using Disney.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Services
{
    public interface IUserService
    {
        public Task<UserManagerResponse> RegisterUserAsync(RegisterModel model);
        public Task<UserManagerResponse> LoginUserAsync(LoginModel model);
    }
}
