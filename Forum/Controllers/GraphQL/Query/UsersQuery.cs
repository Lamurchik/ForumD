using Forum.Model.DB;
using HotChocolate.Authorization;

namespace Forum.Controllers.GraphQL.Query
{
    [ExtendObjectType(typeof(Query))]
    public class UsersQuery
    {
        [AllowAnonymous]
        public  IQueryable<User> GetUser ([Service] ForumDBContext context)
        {
            return context.Users;
        }             

    }
}
