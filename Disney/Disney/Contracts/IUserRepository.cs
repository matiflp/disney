using Disney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Contracts
{
    public interface IUserRepository
    {
        public User FindByEmail(string email);
        public void Save(User user);
    }
}
