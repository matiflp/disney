using Disney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Auth
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, User user);
        bool IsTokenValid(string key, string issuer, string token);
    }
}
