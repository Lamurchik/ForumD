using Forum.Model;
using Forum.Model.Services;
using GreenDonut;
using HotChocolate.Authorization;


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
        public async Task<string> SignIn([Service] IAuthorizationService authorization, string email, string password)
        {
            string jwt;
            try
            {
                 jwt = await authorization.Login(email, password);

            }
            catch (Exception ex)
            { return ex.Message; }

            return jwt;
        }


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
        
    

        [Authorize(Roles = new[] {RoleAndPoliceName.admin })]
        public string AdmenTestHotCH()
        {
            return "you admen";
        }

    }

    
    }
