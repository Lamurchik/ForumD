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
        public int UserId { get; set; }

        public  Post? Post { get; set; }
        public Comment? Comment { get; set; }
        public required User User { get; set; }
        public required bool IsLike  { get; set; }
    }
}
