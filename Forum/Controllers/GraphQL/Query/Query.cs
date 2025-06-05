using Forum.Model.DB;
using Forum.Model.Services;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers.GraphQL.Query
{
   
    public class Query
    {
        public async Task<OutputGrade> GradeCounter([Service] IGrateService grateService, int? postId, int? commentId)
        {
            if(postId != null && commentId!=null)
                throw new ArgumentException("choose one thing");
            if(postId!= null)
            {
                return await grateService.CounterPost((int)postId);
            }
            if(commentId!= null)
                return await grateService.CounterComment((int)commentId);

            throw new ArgumentException("please specify the ID");
        }
        [UseProjection]
        [UseSorting]
        public IQueryable<Post> Search([Service] SearchService searchService, string searchQuery)
        { 
            return searchService.Search(searchQuery);
        }
        [UseProjection]
        public IQueryable<Post> PostDay([Service] SearchService searchService)
        {
            return searchService.GetLatestPosts();   
        }
        
    }
   
}
