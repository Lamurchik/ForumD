using Forum.Model.DB;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Forum.Model.Services
{
    public interface ICommentManager
    {
        //public  Task<string>


        public  Task<string> AddComment();

        public  Task<string> ChangeComment();


        public Task<string> DeleteComment();

    }

    public class InputComment
    {
        
        public int Id { get; set; } // Уникальный идентификатор комментария
        
        public required string Body { get; set; } // Текст комментария
        public string? Image { get; set; } //имя файла;
        public int? ParentCommentId { get; set; } // Идентификатор родительского комментария (если это ответ)
        public int UserId { get; set; } // Идентификатор пользователя, оставившего комментарий      
        public int PostId { get; set; } // Идентификатор поста, к которому относится комментарий

        public DateTime CommentDate { get=> DateTime.UtcNow.Date;  } // Дата комментария     
        public TimeOnly CommentTime { get=> TimeOnly.FromDateTime(DateTime.Now);  } // Время комментария

    }


    public class CommentManagerService
    {


    }
}
