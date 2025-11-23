using WorkConnect.Api.Hateoas;

namespace WorkConnect.Api.Dtos
{
    public class TipCreateDto
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Category { get; set; }
        public int AuthorId { get; set; }
    }

    public class TipUpdateDto
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Category { get; set; }
    }

    public class TipResponseDto
    {
        public int Id { get; set; }              
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }

        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }

        public List<LinkDto> Links { get; set; } = new();
    }
}
