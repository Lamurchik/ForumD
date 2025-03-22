using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.DB
{
    public class PostPartial
    {
        public int Id { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }

        [GraphQLIgnore]
        public  Post? Post { get; set; }
        public PostPartialType PostPartialType { get; set; } //в бд будет отдельная бд для enum 
        public required string Body { get; set; }
        public int Order {  get; set; } // порядок кусочков 
    }
}
