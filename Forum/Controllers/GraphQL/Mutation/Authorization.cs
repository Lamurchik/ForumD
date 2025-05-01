using Forum.Model;
using Forum.Model.Services;
using GreenDonut;
using HotChocolate.Authorization;
using System.Security.Claims;


namespace Forum.Controllers.GraphQL.Mutation
{
    [ExtendObjectType(typeof(Mutation))]
    public class Authorization
    {
        public async  Task<string> SignUp([Service] IAuthorizationService authorization, string nickName, string email, string password)
        {
            try 
            {
              bool result = await authorization.Register(nickName, email, password);
            
            }
            catch(Exception ex) 
            { return ex.Message; }
            return "register successful";
        }
        public async Task<LoginAnswer> SignIn([Service] IAuthorizationService authorization, string email, string password)
        {
            LoginAnswer answer = new LoginAnswer();
            try
            {
                answer = await authorization.Login(email, password);

            }
            catch (Exception ex)
            { 
                answer.Message = ex.Message;
                return answer; 
            }

            return answer;
        }
        //только админ
        [Authorize(Roles = new[] { RoleAndPoliceName.admin })]
        public async Task<bool> ChangeRole([Service] IAuthorizationService authorization, int userID, int roleId)
        {
            bool result;
            try
            {
                result = await authorization.ChangeRole(userID, roleId);
            }
            catch 
            { return false; }
            return result;
        }

        [Authorize]
        public string RefrashJwt([Service] IAuthorizationService authorization, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            // Здесь можешь использовать userId/role для выдачи нового токена
            return authorization.RefrashJwt(role, userId);
        }


        [Authorize(Roles = new[] {RoleAndPoliceName.admin })]
        public string AdmenTestHotCH()
        {
            return "you admen";
        }

    }

    
    }
