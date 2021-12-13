using Disney.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {

        public UserRepository(DisneyContext disneyContext) : base(disneyContext) 
        {
            //users.Add(new UserDTO
            //{
            //    Email = "joydipkanjilal@gmail.com",
            //    Password = "joydip123",
            //    Role = "manager"
            //});
            //users.Add(new UserDTO
            //{
            //    Email = "michaelsanders@gmail.com",
            //    Password = "michael321",
            //    Role = "developer"
            //});
            //users.Add(new UserDTO
            //{
            //    Email = "stephensmith@gmail.com",
            //    Password = "stephen123",
            //    Role = "tester"
            //});
            //users.Add(new UserDTO
            //{
            //    Email = "rodpaddock@gmail.com",
            //    Password = "rod123",
            //    Role = "admin"
            //});
            //users.Add(new UserDTO
            //{
            //    Email = "rexwills@gmail.com",
            //    Password = "rex321",
            //    Role = "admin"
            //});
        }

        public User FindByEmail(string email)
        {
            return FindByCondition(user => user.Email == email).FirstOrDefault();
        }

        public User GetUser(UserDTO userModel)
        {
            return FindByCondition(user => user.UserName == userModel.UserName
                && user.Password == userModel.Password).FirstOrDefault();
        }

        public void Save(User user)
        {
            Create(user);
            SaveChanges();
        }
    }
}
