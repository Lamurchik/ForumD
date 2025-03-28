using Forum.Model.DB;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Path = System.IO.Path;
using Microsoft.EntityFrameworkCore;

namespace Forum.Model.Services
{
    public interface ICommentManager
    {
        //public  Task<string>

        public  Task<string> AddCommentAsync(InputComment ic, IFile? file);

        public  Task<string> ChangeCommentAsync(int commId, string? body, IFile? file);

        public Task<string> DeleteCommentAsync(int comId);

    }
    
    public class InputComment
    {       
        public required string Body { get; set; } // Текст комментария
        //public string? Image { get; set; } //имя файла;
        public int? ParentCommentId { get; set; } // Идентификатор родительского комментария (если это ответ)
        public int UserId { get; set; } // Идентификатор пользователя, оставившего комментарий      
        public int PostId { get; set; } // Идентификатор поста, к которому относится комментарий

        public static DateTime CommentDate { get=> DateTime.UtcNow.Date;  } // Дата комментария     
        public static TimeOnly CommentTime { get=> TimeOnly.FromDateTime(DateTime.Now);  } // Время комментария
    }


    public class CommentManagerService : ICommentManager
    {
        ForumDBContext _dbContext;

        public CommentManagerService(ForumDBContext dbContext)
        {
            _dbContext = dbContext;
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
       

        public async Task<string> AddCommentAsync(InputComment ic, IFile? file)
        {
            Comment comment = new Comment()
            { 
                Body = ic.Body,
                Image = file ==null ? null : await SaveFileAsync(file),
                ParentCommentId = ic.ParentCommentId,
                UserId = ic.UserId,
                PostId = ic.PostId,
                CommentDate = InputComment.CommentDate,
                CommentTime = InputComment.CommentTime            
            };

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            return $"comment added whit id:{comment.Id}";
        }

        public async Task<string> ChangeCommentAsync(int commId,string? body, IFile? file)
        {
            var comm = await _dbContext.Comments.FirstOrDefaultAsync(c=> c.Id==commId);
            if (comm == null) throw new ArgumentException("comment not found");

            if (body != null) comm.Body = body;
            if(file!= null)
            {
                if(comm.Image!=null)
                DeleteFile(comm.Image);
                comm.Image = await SaveFileAsync(file);
            }
            await _dbContext.SaveChangesAsync();

            return $"comment update";
        }

        public async Task<string> DeleteCommentAsync(int commId)
        {
            var comm = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commId);
            if(comm == null) throw new ArgumentException("comment not found");
            _dbContext.Comments.Remove(comm);

            await _dbContext.SaveChangesAsync();
            return "comment Delete";
        }
    }
}
