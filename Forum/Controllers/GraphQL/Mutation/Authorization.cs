using Forum.Model;
using GreenDonut;

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
            string result;
            try
            {
                 result = await authorization.Login(email, password);
                

            }
            catch (Exception ex)
            { return ex.Message; }



            return result;


        }
    }
}
