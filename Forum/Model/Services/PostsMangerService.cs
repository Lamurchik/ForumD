using Forum.Model.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using Path = System.IO.Path;

namespace Forum.Model.Services
{
    public interface IPostsMangerService
    {
    
    
    
    }

    


    public class PostsMangerService
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


        #region update

        public async Task<string> PostTitleChange(int postId, string newTitle)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null) throw new ArgumentException("Invalid post ID");

            post.Title = newTitle;
            await _dbContext.SaveChangesAsync();
            return "successfully changed";
        }

        public async Task<string> PostPartialChange(int postId, string? newBody, IFile? newFile)
        {
            if (newBody == null && newFile == null)
                throw new ArgumentException("body and file can't be null at the same time");

            var partial = await _dbContext.PostPartials.FirstOrDefaultAsync(p => p.Id == postId);
            if (partial == null)
                throw new ArgumentException("Invalid post ID");


            if (newFile != null)
            {
                DeleteFile(partial.Body);
                partial.Body = await SaveFileAsync(newFile);
            }
            else 
            {
                partial.Body = newBody!;
            }

   
            await _dbContext.SaveChangesAsync();
            return "successfully changed";
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

            return fileName; // Возвращаем абсолютный путь
        }
        #endregion

        #region создание поста

        public async Task<string> PostCreate(string title, int userId, string keyPostPartials)
        {
             var cacheData   = await _cache.GetStringAsync(keyPostPartials);
            if(cacheData ==null) throw new ArgumentException("Invalid key value");
            try 
            {
                List<PostPartial>? postPartials = JsonSerializer.Deserialize<List<PostPartial>>(cacheData);
                return await PostCreate(title, userId, postPartials!);
            }
            catch
            {
                throw new ArgumentException("Invalid key value or data cache");
            }           
        }

        private async Task<string> PostCreate(string title, int userId, ICollection <PostPartial>  postPartials)
        {
            Post post = new Post() 
            {
                Title =title,
                UserAuthorId = userId,
                PostPartials = new List<PostPartial>(),
                DateCreate = DateTime.Today,
                TimeCreate = TimeOnly.FromDateTime(DateTime.Now)
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


        public async Task<string> PostPartialsCreate(string? clientKey, int postId, int order, string? body = null, IFile? file = null)
        {
            if (body == null && file == null)
                throw new ArgumentException("body and file can't be null at the same time");

            PostPartial pp = new PostPartial()
            {
                PostId = postId,
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
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) // Настройте время хранения в кэше
            });
            return key;
        }
        #endregion
    }
}
