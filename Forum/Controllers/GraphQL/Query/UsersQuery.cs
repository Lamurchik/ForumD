using Forum.Model.DB;
using HotChocolate.Authorization;

namespace Forum.Controllers.GraphQL.Query
{
    [ExtendObjectType(typeof(Query))]
    public class UsersQuery
    {

        [AllowAnonymous]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public  IQueryable<User> GetUsers ([Service] ForumDBContext context)
        {
            return context.Users;
        }
    }
}
