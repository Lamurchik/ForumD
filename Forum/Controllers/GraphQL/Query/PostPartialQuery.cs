using Forum.Model.DB;

namespace Forum.Controllers.GraphQL.Query
{
    [ExtendObjectType(typeof(Query))]
    public class PostPartialQuery
    {

        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IQueryable<PostPartial> GetPostsPartial([Service] ForumDBContext context)
        {
            return context.PostPartials;
        }
    }
}
