using Forum.Model.DB;
using Microsoft.EntityFrameworkCore;
using System.Data;
using BCrypt.Net;

namespace Forum.Model
{
    public class AuthorizationService
    {
        private ForumDBContext _dbContext;

        public AuthorizationService(ForumDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<string> Login()
        {
            return null;
        }

        //добавить кэш к Role
        public async Task<bool>  Register(string nickName, string email, string password) 
        {
            User? u = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u=> u.NickName==nickName);

            if (u != null)
            {
                throw new ArgumentException("This Name is already taken");
            }



            Role? userRole = await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(role => role.RoleName == RoleAndPoliceName.user);
            if (userRole == null) 
            {
                throw new InvalidOperationException("User role not found");

                //return false;                
            }
           

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);


            User user = new User { Email = email, Password = hashPassword, NickName= nickName, Role = userRole };
            
           _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();


            return true;
        }


        public async Task<bool> ChangeRole(int userId, int roleId)
        {
            return false;
        }




    }
}
