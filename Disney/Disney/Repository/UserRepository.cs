using Disney.Contracts;
using Disney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Repository
{
    public class UserRepository : RepositoryBase<User>,  IUserRepository
    {

        public UserRepository(DisneyContext disneyContext) : base(disneyContext)
        {
        }

        public User FindByEmail(string email)
        {
            return FindByCondition(user => user.Email == email).FirstOrDefault();
        }

        public void Save(User user)
        {
            Create(user);
            SaveChanges();
        }
    }
}
