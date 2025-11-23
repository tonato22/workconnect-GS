namespace WorkConnect.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }          
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Occupation { get; set; }
        public string? Country { get; set; }
        public string? ExperienceLevel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Tip> Tips { get; set; } = new List<Tip>();
    }
}
