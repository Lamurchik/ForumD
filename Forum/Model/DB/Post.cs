using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.DB
{
    public class Post
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateTime? DateCreate { get; set; }
        public TimeOnly TimeCreate { get; set; }

        [ForeignKey("User")]
        public int UserAuthorId { get; set; } // Идентификатор автора поста (ссылается на пользователя)
        public required User User { get; set; }
        public required ICollection<PostPartial> PostPartials { get; set; }
        public ICollection<Grade>? Grades { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        public ICollection<Tags>? Tags { get; set; }
    }
}
