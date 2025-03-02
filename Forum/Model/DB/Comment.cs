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
        public int UserId { get; set; } // Идентификатор пользователя, оставившего комментарий

        [ForeignKey("Post")]
        public int PostId { get; set; } // Идентификатор поста, к которому относится комментарий
        public required User User { get; set; }
        public required Post Post { get; set; }

        public DateTime CommentDate { get; set; } // Дата комментария     
        public TimeOnly CommentTime { get; set; } // Время комментария

    }
}
