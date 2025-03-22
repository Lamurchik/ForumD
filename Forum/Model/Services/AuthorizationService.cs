using Forum.Model.DB;
using Microsoft.EntityFrameworkCore;
using System.Data;
using BCrypt.Net;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Forum.Model.Services
{

    public interface IAuthorizationService
    {
        public Task<string> Login(string email, string password);

        public Task<bool> Register(string nickName, string email, string password);

        public Task<bool> ChangeRole(int userId, int roleId);

    }


    public class AuthorizationService : IAuthorizationService
    {
        private ForumDBContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthorizationService(ForumDBContext dbContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }


        public async Task<string> Login(string email, string password)
        {
            User? user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new ArgumentException("Неверный email или пароль");
            }

            var role = user.Role.RoleName;
            string jwt = GenerateJwtToken(user, user.Role.RoleName);

            return jwt;
        }

        //добавить кэш к Role
        public async Task<bool>  Register(string nickName, string email, string password) 
        {
            User? u = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u=> u.NickName==nickName);

            if (u != null)
            {
                throw new ArgumentException("This Name is already taken");
            }



            Role? userRole = await _dbContext.Roles.FirstOrDefaultAsync(role => role.RoleName == RoleAndPoliceName.user);
            if (userRole == null) 
            {
                throw new InvalidOperationException("User role not found");

                //return false;                
            }
           

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);


            User user = new User { Email = email, Password = hashPassword, NickName= nickName, Role = userRole };
            
            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            Console.WriteLine("register is work");
            return true;
        }


        public async Task<bool> ChangeRole(int userId, int roleId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new ArgumentException("Unknown user ID");
            }

            var roleExists = await _dbContext.Roles.AsNoTracking().AnyAsync(r => r.Id == roleId);
            if (!roleExists)
            {
                throw new ArgumentException("Invalid role ID");
            }

            user.RoleId = roleId;
            await _dbContext.SaveChangesAsync();

            return true;
        }


        private string GenerateJwtToken(User user, string role)
        {
            var claims = new[]
            {
           // new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid( ).ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())

            };
            //var q = _configuration["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
