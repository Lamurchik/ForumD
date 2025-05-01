using Forum.Model;
using Forum.Model.DB;
using HotChocolate.Types;
using HotChocolate.Data;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

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
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IQueryable<Post> GetPostsPaging([Service] ForumDBContext context)
        {
            return context.Posts;
        }


    }

    /*
     логика атрибута 
    создаём ключ на основе запроса 
    сохраняем то что получили бы от  return context.Posts;
    применяеи до пагинации - цель избавиться от запроса к бд, а не кэшировать весь ответ  
     
     */




}
