using Forum.Model.DB;

namespace Forum.Controllers.GraphQL.Query
{
    [ExtendObjectType(typeof(Query))]
    public class UsersQuery
    {
        public  IQueryable<User> GetUser ([Service] ForumDBContext context)
        {
            return context.Users;
        }             

    }
}
