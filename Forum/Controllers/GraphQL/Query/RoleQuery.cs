using Forum.Model.DB;

namespace Forum.Controllers.GraphQL.Query
{
    [ExtendObjectType(typeof(Query))]
    public class RoleQuery
    {        
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IQueryable<Role> GetRole([Service] ForumDBContext context)
        {
            return context.Roles;
        }
    }
}
