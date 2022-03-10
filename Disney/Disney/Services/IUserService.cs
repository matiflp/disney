using Disney.Models.Auth;
using System.Threading.Tasks;

namespace Disney.Services
{
    public interface IUserService
    {
        public Task<UserManagerResponse> RegisterUserAsync(RegisterModel model);
        public Task<UserManagerResponse> LoginUserAsync(LoginModel model);
    }
}
