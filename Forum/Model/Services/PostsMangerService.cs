using Forum.Model.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json;
using Path = System.IO.Path;

namespace Forum.Model.Services
{
    public interface IPostsMangerService
    {
        public Task<string> PostCreateAsync(PostInput postInput, IFile? titleImage);

        public Task<string> PostUpdateAsync(int postId, string? Body, string? title, IFile? titleImage);

        public Task<string> PostDeleteAsync(int postId);

        public string GetFilePath(string fileName);

    }

    public class PostInput
    {       
        public required string Title { get; set; }
        public static DateTime Date { get => DateTime.UtcNow.Date; } // Дата   
        public static TimeOnly Time { get => TimeOnly.FromDateTime(DateTime.Now); } // Время 
        [ForeignKey("User")]
        public required int UserAuthorId { get; set; } // Идентификатор автора поста (ссылается на пользователя)
        public required string Body { get; set; }
      //  public string? Image { get; set; }
     
    }



    public class PostsMangerService : IPostsMangerService
    {

        private ForumDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private IDistributedCache _cache;
        

        public PostsMangerService(ForumDBContext dbContext, IConfiguration configuration, IDistributedCache cache)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _cache = cache;
        }


        public async Task<string> PostCreateAsync(PostInput postInput, IFile? titleImage)
        {
            var post = new Post()
            {
                Title = postInput.Title,
                UserAuthorId = postInput.UserAuthorId,
                Body = postInput.Body,
                TitleImage = titleImage ==null ? null : await SaveFileAsync(titleImage),
                DateCreate = PostInput.Date,
                TimeCreate = PostInput.Time                
            };
            _dbContext.Posts.Add(post);

            await _dbContext.SaveChangesAsync();


            return "post added";

        }

        public async Task<string> PostUpdateAsync(int postId, string? Body, string? title, IFile? titleImage)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) throw new ArgumentException("object not found");


            //нужен анализ тела при редактирование картинки 
            if(Body!=null) post.Body = Body;
            if(title!=null) post.Title = title;
            if(titleImage!=null)
            {
                if(post.TitleImage!=null) DeleteFile(post.TitleImage);
                post.TitleImage = await SaveFileAsync(titleImage);
            }

            await _dbContext.SaveChangesAsync();

            return "postChange";
        }

        public async Task<string> PostDeleteAsync(int postId)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) throw new ArgumentException("object not found");
            //произойдёт ли коскадное удаление
            _dbContext.Posts.Remove(post!);

            await _dbContext.SaveChangesAsync();

            return "post deleted";
        }

        #region вспомогательные методы 
        public string GetFilePath(string fileName)
        {
            return Path.Combine(AppContext.BaseDirectory, "wwwroot", "Images", fileName);
        }
        private async Task<string> SaveFileAsync(IFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Файл пуст или не был загружен.");
            }

            string imageDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot", "Images");
            Directory.CreateDirectory(imageDirectory); // Создаём папку, если её нет

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}"; // Уникальное имя
            string filePath = Path.Combine(imageDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
        public bool DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    return true;
                }
                catch (Exception ex)
                {
                    // Логирование ошибки (если нужно)
                    Console.WriteLine($"Ошибка при удалении файла: {ex.Message}");
                    return false;
                }
            }
            else
            {
                // Файл не найден
                return false;
            }
        }



        #endregion





        //не актуальыные методы 
        /*
        #region delete


        public async Task<string> DeletePostAsync(int postId)
        {

            var post = await _dbContext.Posts.FirstOrDefaultAsync(p=>p.Id==postId);

            if (post == null) throw new ArgumentException("object not found");
            //произойдёт ли коскадное удаление
            _dbContext.Posts.Remove(post!);

            await _dbContext.SaveChangesAsync();

            return "post deleted";
        }

        public async Task<string> DeletePostPartialAsync(int partialId)
        {
            var partial = await _dbContext.PostPartials.FirstOrDefaultAsync(p => p.Id == partialId);
            if (partial == null) throw new ArgumentException("object not found");

            _dbContext.PostPartials.Remove(partial!);

            await _dbContext.SaveChangesAsync();


            return "partial delated";
        }

        #endregion

        #region update

        public async Task<string> PostTitleChangeAsync(int postId, string newTitle)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null) throw new ArgumentException("Invalid post ID");

            post.Title = newTitle;
            await _dbContext.SaveChangesAsync();
            return "successfully changed";
        }

        public async Task<string> PostPartialChangeAsync(int postPartialID, string? newBody, IFile? newFile)
        {
            if (newBody == null && newFile == null)
                throw new ArgumentException("body and file can't be null at the same time");

            var partial = await _dbContext.PostPartials.FirstOrDefaultAsync(p => p.Id == postPartialID);
            if (partial == null)
                throw new ArgumentException("Invalid post ID");


            if (newFile != null)
            {
                if(partial.PostPartialType== PostPartialType.Image)
                        DeleteFile(partial.Body);
                partial.Body = await SaveFileAsync(newFile);
            }
            else 
            {
                if (partial.PostPartialType == PostPartialType.Image)
                    DeleteFile(partial.Body);
                partial.Body = newBody!;
            }

   
            await _dbContext.SaveChangesAsync();
            return "successfully changed";
        }

        public async Task<string> AddTagsAsync(int postId, params Tags [] tags )
        {
            var post = await _dbContext.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null) throw new ArgumentException("post not found");
            List<Tags>? addTags = post.Tags as List<Tags>;

            foreach(var t in tags)
            {
                var bdTag = await _dbContext.Tags.FirstOrDefaultAsync(tg => tg.Name == t.Name);
                if(bdTag!=null)
                {
                    addTags!.Add(bdTag);
                }
                else addTags!.Add(t);
            }

            post.Tags = addTags;

            await _dbContext.SaveChangesAsync();
            return "Tags added";
        }


        public async Task<string> AddPartialAsync(int postId, int afterOrder, string? body, IFile? file)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Проверяем существование поста
                bool postExists = await _dbContext.Posts.AnyAsync(p => p.Id == postId);
                if (!postExists)
                    throw new ArgumentException("Post not found");

                // Сдвигаем `Order` у всех частичных блоков, начиная с `afterOrder + 1`
                await _dbContext.PostPartials
                    .Where(p => p.PostId == postId && p.Order > afterOrder)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Order, p => p.Order + 1));

                // Создаём новый кусок
                var newPartial = new PostPartial
                {
                    PostId = postId,
                    Order = afterOrder + 1,
                    PostPartialType = file == null ? PostPartialType.Text : PostPartialType.Image,
                    Body = file == null ? body ?? string.Empty : await SaveFileAsync(file)
                };

                _dbContext.PostPartials.Add(newPartial);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync(); // Завершаем транзакцию
                return $"Partial added with id:{newPartial.Id}";
            }
            catch
            {
                await transaction.RollbackAsync(); // Откат в случае ошибки
                throw;
            }
        }

        #endregion

        

        #region создание поста


        /*
        public async Task<string> AddTags(int postId, params Tags[] tags)
        {
            var post = await _dbContext.Posts
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                throw new ArgumentException("Post not found");

            var tagNames = tags.Select(t => t.Name).ToHashSet(); // Убираем дубли в запросе
            var existingTags = await _dbContext.Tags
                .Where(t => tagNames.Contains(t.Name))
                .ToDictionaryAsync(t => t.Name);

            var newTags = tagNames
                .Except(existingTags.Keys) // Фильтруем уже существующие
                .Select(name => new Tags { Name = name })
                .ToList();

            if (newTags.Count > 0)
            {
                await _dbContext.Tags.AddRangeAsync(newTags);
                await _dbContext.SaveChangesAsync();
            }

            // Обновляем existingTags после добавления новых данных
            foreach (var tag in newTags)
            {
                existingTags[tag.Name] = tag;
            }

            // Добавляем только **новые** теги в пост
            foreach (var tag in existingTags.Values)
            {
                if (!post.Tags.Any(t => t.Id == tag.Id)) // Проверяем по Id, а не по Name
                {
                    post.Tags.Add(tag);
                }
            }

            await _dbContext.SaveChangesAsync();
            return "Tags added";
        }
        






        public async Task<string> PostCreateAsync(string title, int userId, string keyPostPartials, IFile? file)
        {
             var cacheData   = await _cache.GetStringAsync(keyPostPartials);
            if(cacheData ==null) throw new ArgumentException("Invalid key value");
            try 
            {
                List<PostPartial>? postPartials = JsonSerializer.Deserialize<List<PostPartial>>(cacheData);
                return await PostCreateAsync(title, userId, postPartials!, file);
            }
            catch
            {
                throw new ArgumentException("Invalid key value or data cache");
            }           
        }

        private async Task<string> PostCreateAsync(string title, int userId, ICollection <PostPartial>  postPartials, IFile? file)
        {
            Post post = new Post()
            {
                Title = title,
                UserAuthorId = userId,
                PostPartials = new List<PostPartial>(),
                DateCreate = DateTime.UtcNow.Date,
                TimeCreate = TimeOnly.FromDateTime(DateTime.Now),
                Image = file != null ? await SaveFileAsync(file) : null

            }; 

           

            foreach (var partial in postPartials)
            {
                partial.Post = post;                 
                post.PostPartials.Add(partial);
            }


            try
            {
                _dbContext.Posts.Add(post);

                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return "post added";           
        }

       /// <summary>
       /// пользователь поэтапно создаёт сложный объект в памяти сервера, получая в ответ ключ по которому можно получить этот объект 
       /// </summary>
       /// <param name="clientKey"></param>
       /// <param name="postId"></param>
       /// <param name="order"></param>
       /// <param name="body"></param>
       /// <param name="file"></param>
       /// <returns></returns>
       /// <exception cref="ArgumentException"></exception>
        public async Task<string> PostPartialsCreateAsync(string? clientKey, int order, string? body = null, IFile? file = null)
        {
            if (body == null && file == null)
                throw new ArgumentException("body and file can't be null at the same time");

            PostPartial pp = new PostPartial()
            {
                PostId = 0,
                Order = order,
                PostPartialType = file == null ? PostPartialType.Text : PostPartialType.Image,
                Body = file == null ? body! : await SaveFileAsync(file)
            };
            string key;
            string? cache = null;
            List<PostPartial>? postPartials;

            if (clientKey != null)
            {
                cache = await _cache.GetStringAsync(clientKey);
            }

            if (cache != null)
            {
                key = clientKey!;
                postPartials = JsonSerializer.Deserialize<List<PostPartial>>(cache);
                if (postPartials == null) 
                    postPartials = new List<PostPartial>(); 
            }
            else
            {
                key = $"{Guid.NewGuid()}:Partials";
                postPartials = new List<PostPartial>();
            }
            postPartials.Add(pp);
            cache = JsonSerializer.Serialize(postPartials);

            await _cache.SetStringAsync(key, cache, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4) // Настройте время хранения в кэше
            });
            return key;
        }
        #endregion
        */
    }
}
