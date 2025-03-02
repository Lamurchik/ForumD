namespace Forum.Model.DB
{
    public class Tags
    {   
        public int Id { get; set; }       
        public required string Name { get; set; }
        public ICollection<Post>? Posts { get; set; }

        public Category? Category { get; set; }
    }
}
