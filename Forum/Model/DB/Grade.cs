using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.DB
{
    public class Grade
    {
        public int Id { get; set; }
        [ForeignKey("Post")]
        public int? PostId { get; set; }
        [ForeignKey("Comment")]
        public int? CommentId { get; set; }
        [ForeignKey("User")]
        public required int UserId { get; set; }
        public  Post? Post { get; set; }
        public Comment? Comment { get; set; }
        public  User? User { get; set; }
        public required bool IsLike  { get; set; }

        public DateTime GradeDate { get; set; } // Дата комментария     
        public TimeOnly GradeTime { get; set; } // Время комментария
    }
}
