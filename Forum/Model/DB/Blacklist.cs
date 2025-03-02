namespace Forum.Model.DB
{
    public class Blacklist
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // Если блокируется пользователь
        public string? IPAddress { get; set; } // Если блокируется IP
        public DateTime BlockedUntil { get; set; }
    }
}
