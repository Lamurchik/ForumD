namespace Forum.Model.DB
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Tags>? Tags { get; set; }
    }
}
