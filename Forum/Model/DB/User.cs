using System.Text.Json.Serialization;

namespace Forum.Model.DB
{
    public class User
    {
        public int Id { get; set; }
        public required string NickName { get; set; }
        public required string Email { get; set; }
        [GraphQLIgnore]
        public required string Password { get; set; }
        public int RoleId { get; set; }
        public required Role Role { get; set; }

        /*
        public ICollection< Subscriptions>? Subscriptions { get; set; }

        public ICollection<Subscriptions>? Subscribers { get; set; }
        */


        public ICollection<Post>? Posts { get; set; }
        public UserInfo? UserInfo { get; set; }
    }
}
