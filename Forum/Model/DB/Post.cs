using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.DB
{
    //сделать превью
    public class Post
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        
        public DateTime? DateCreate { get; set; }
        
        public TimeOnly TimeCreate { get; set; }

        [ForeignKey("User")]
        public required int UserAuthorId { get; set; } // Идентификатор автора поста (ссылается на пользователя)
        public  User? User { get; set; }

        [UseSorting]
        public required ICollection<PostPartial> PostPartials { get; set; }
        public ICollection<Grade>? Grades { get; set; }

        [UseFiltering]
        [UseSorting]
        public ICollection<Comment>? Comments { get; set; }

        public ICollection<Tags>? Tags { get; set; }
    }
}
