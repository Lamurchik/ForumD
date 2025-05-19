using Forum.Model;
using Forum.Model.DB;
using HotChocolate.Types;
using HotChocolate.Data;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;

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
        //при пагинации не рабтает кэш, я не знаю как пофиксить, слишком много проблем
        // написать отдельную логику ?
        //создать кастомный атрибут 
        // оставляю как есть. это не возможно
        // написать свою реализацию 
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IQueryable<Post> GetPostsPaging([Service] ForumDBContext context)
        {
            try
            {
                return context.Posts;
            }
            catch (Exception ex)             
                { throw new Exception(ex.Message);  }
        }

        [UseProjection]
        public IQueryable<Post> GetPostsPagingV2([Service] ForumDBContext context, int userId, int pageNumber = 1, int pageSize =10 )
        {                  
            return context.Posts.Where(p => p.UserAuthorId == userId).OrderBy(p=>p.DateCreate).Skip((pageNumber-1)*pageSize).Take(pageSize);       
        }




    }

    /*
     логика атрибута 
    создаём ключ на основе запроса 
    сохраняем то что получили бы от  return context.Posts;
    применяеи до пагинации - цель избавиться от запроса к бд, а не кэшировать весь ответ  
     
     */




}
