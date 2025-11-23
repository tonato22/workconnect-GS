namespace WorkConnect.Domain.Entities
{
    public class Tip
    {
        public int Id { get; set; }              
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int AuthorId { get; set; }
        public User? Author { get; set; }
    }
}
