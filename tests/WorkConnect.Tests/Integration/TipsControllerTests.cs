using System.Net;
using System.Net.Http.Json;
using WorkConnect.Api.Dtos;
using Xunit;

namespace WorkConnect.Tests.Integration
{
    public class TipsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TipsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTips_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/v1/tips");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateTip_ShouldReturnCreated_WhenAuthorExists()
        {
            // Cria um usuário
            var newUser = new UserCreateDto
            {
                Name = "Author Test",
                Email = "author@example.com",
                Occupation = "Engineer",
                Country = "Brazil",
                ExperienceLevel = "Pleno"
            };

            var userResponse = await _client.PostAsJsonAsync("/api/v1/users", newUser);
            Assert.Equal(HttpStatusCode.Created, userResponse.StatusCode);

            var createdUser = await userResponse.Content.ReadFromJsonAsync<UserResponseDto>();
            Assert.NotNull(createdUser);

            var newTip = new TipCreateDto
            {
                Title = "Dica de foco",
                Content = "Use blocos de 25 minutos para estudar.",
                Category = "Produtividade",
                AuthorId = createdUser!.Id
            };

            var tipResponse = await _client.PostAsJsonAsync("/api/v1/tips", newTip);
            Assert.Equal(HttpStatusCode.Created, tipResponse.StatusCode);

            var createdTip = await tipResponse.Content.ReadFromJsonAsync<TipResponseDto>();
            Assert.NotNull(createdTip);
            Assert.True(createdTip!.Id > 0);
            Assert.Equal(createdUser.Id, createdTip.AuthorId);
        }
    }
}
