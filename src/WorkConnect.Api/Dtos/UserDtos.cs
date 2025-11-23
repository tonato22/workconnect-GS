namespace WorkConnect.Api.Dtos
{
    public class UserCreateDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Occupation { get; set; }
        public string? Country { get; set; }
        public string? ExperienceLevel { get; set; }
    }

    public class UserUpdateDto
    {
        public string Name { get; set; } = null!;
        public string? Occupation { get; set; }
        public string? Country { get; set; }
        public string? ExperienceLevel { get; set; }
    }

    public class UserResponseDto
    {
        public int Id { get; set; }          
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Occupation { get; set; }
        public string? Country { get; set; }
        public string? ExperienceLevel { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
