using System.Net;
using System.Net.Http.Json;
using WorkConnect.Api.Dtos;
using Xunit;

namespace WorkConnect.Tests.Integration
{
    public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UsersControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/v1/users");

            // Substituindo FluentAssertions por Assert do xUnit
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreated()
        {
            var newUser = new UserCreateDto
            {
                Name = "Test User",
                Email = "testuser@example.com",
                Occupation = "Developer",
                Country = "Brazil",
                ExperienceLevel = "Junior"
            };

            var response = await _client.PostAsJsonAsync("/api/v1/users", newUser);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<UserResponseDto>();

            Assert.NotNull(created);
            Assert.True(created!.Id > 0);
            Assert.Equal(newUser.Email, created.Email);
        }
    }
}
