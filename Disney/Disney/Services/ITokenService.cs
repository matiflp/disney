using Disney.Models;
using Disney.Models.Auth;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Services
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, LoginModel user);
        bool IsTokenValid(string key, string issuer, string token);
    }
}
