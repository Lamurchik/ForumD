using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.DB
{
    public class SocialNotification
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        public required string Message { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }

       
        public required Post Post { get; set; }

       // public required string Link { get; set; }// нужно ли?

        public required DateTime Date { get; set; }

        public TimeOnly Time { get; set; }
    }
}
