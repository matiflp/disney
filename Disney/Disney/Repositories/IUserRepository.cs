using Disney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Repositories
{
    public interface IUserRepository
    {
        public User FindByEmail(string email);
        User GetUser(UserDTO userModel);
        public void Save(User user);
    }
}
