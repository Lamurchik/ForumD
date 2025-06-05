using Forum.Model.DB;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers.GraphQL.Query
{
    [ExtendObjectType(typeof(Query))]
    public class CommentQuery
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IQueryable<Comment> GetComments ([Service] ForumDBContext context)
        {
            return context.Comments;
        }
    }
}
