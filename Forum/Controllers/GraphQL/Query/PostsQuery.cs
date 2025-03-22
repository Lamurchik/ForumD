using Forum.Model.DB;

namespace Forum.Controllers.GraphQL.Query
{
    [ExtendObjectType(typeof(Query))]
    public class PostsQuery
    {

        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IQueryable<Post> GetPosts([Service] ForumDBContext context)
        {
            return context.Posts;
        }

    }
}
