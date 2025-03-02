using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.DB
{
    public class UserInfo
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } 

        public required User User { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Country { get; set; }
    }
}
