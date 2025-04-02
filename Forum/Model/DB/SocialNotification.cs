using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.DB
{
    public enum NotificationType
    {
        AnswerComment,
        PostComment,
        GradeScore,
        Subscribe 
    }

    public class SocialNotification
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; } //к какому посту 

        [ForeignKey("User")]
        public int UserId { get; set; } //для кого уведомление 

        [ForeignKey(nameof(Comment))]
        public int? CommentId { get; set; } //если это ответ на коментарий 
        public bool IsRead { get; set; }

        public  Post? Post { get; set; }
        public User? User { get; set; }

        public Comment? Comment { get; set; }

        public NotificationType NotificationType { get; set; }

        public required DateTime Date { get; set; }

        public TimeOnly Time { get; set; }
    }
}
