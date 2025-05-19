using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.DB
{
    public class Comment
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор комментария
        [Required]
        public required string Body { get; set; } // Текст комментария
        public string? Image { get; set; }
        public int? ParentCommentId { get; set; } // Идентификатор родительского комментария (если это ответ)

        [ForeignKey("User")]
        public required int UserId { get; set; } // Идентификатор пользователя, оставившего комментарий

        [ForeignKey("Post")]
        public required int PostId { get; set; } // Идентификатор поста, к которому относится комментарий
        public ICollection<Grade>? Grades { get; set; }
        public  User? User { get; set; }
        public  Post? Post { get; set; }

        public DateTime CommentDate { get; set; } // Дата комментария     
        public TimeOnly CommentTime { get; set; } // Время комментария

        [NotMapped]
        public string GetImage { get => Image != null ? $"https://localhost:7143/api/Image/image/{Image}" : ""; }

    }

    public class CommentType : ObjectType<Comment>
    {
        protected override void Configure(IObjectTypeDescriptor<Comment> descriptor)
        {
            descriptor.BindFieldsExplicitly(); // Без этого HotChocolate не выберет поля автоматически

            // Отображаем обычные поля
            descriptor.Field(c => c.Id);
            descriptor.Field(c => c.Body);
            descriptor.Field(c => c.Image);
            descriptor.Field(c => c.UserId);
            descriptor.Field(c => c.User);
            descriptor.Field(c => c.PostId);
            descriptor.Field(c => c.Post);
            descriptor.Field(c => c.CommentDate);
            descriptor.Field(c => c.CommentTime);
            descriptor.Field(c => c.GetImage);

            descriptor.Field("likesCount")
                .Type<IntType>()
                .Resolve(async ctx =>
                {
                    var db = ctx.Service<ForumDBContext>();
                    var cache = ctx.Service<IDistributedCache>();
                    var comment = ctx.Parent<Comment>();

                    string cacheKey = $"comment:{comment.Id}:likesCount";

                    // Проверка наличия в кэше
                    var cached = await cache.GetStringAsync(cacheKey);
                    if (cached != null)
                    {
                        
                        return int.Parse(cached);
                    }

                    // Запрос в базу
                    int count = await db.Grades
                        .Where(g => g.CommentId == comment.Id && g.IsLike)
                        .CountAsync();

                    // Сохраняем в кэш
                    await cache.SetStringAsync(
                        cacheKey,
                        count.ToString(),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                        });

                    return count;
                });

            descriptor.Field("dislikesCount")
                .Type<IntType>()
                .Resolve(async ctx =>
                {
                    var db = ctx.Service<ForumDBContext>();
                    var cache = ctx.Service<IDistributedCache>();
                    var comment = ctx.Parent<Comment>();

                    string cacheKey = $"comment:{comment.Id}:dislikesCount";

                    var cached = await cache.GetStringAsync(cacheKey);
                    if (cached != null)
                    {
                        return int.Parse(cached);
                    }

                    int count = await db.Grades
                        .Where(g => g.CommentId == comment.Id && !g.IsLike)
                        .CountAsync();

                    await cache.SetStringAsync(
                        cacheKey,
                        count.ToString(),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                        });

                    return count;
                });
        }
    }
}
