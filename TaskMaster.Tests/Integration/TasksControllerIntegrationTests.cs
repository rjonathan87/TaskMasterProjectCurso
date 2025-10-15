using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace TaskMaster.Tests.Integration
{
    // NOTA: Para que 'Program' sea visible aquí, asegúrate de que al final de tu
    // archivo 'TaskMaster.Presentation/Program.cs' tienes la línea:
    // public partial class Program { }
    public class TasksControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TasksControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Usamos el factory para crear un cliente HTTP que hará peticiones a nuestra API en memoria
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllTasks_ReturnsUnauthorized_WhenNoTokenIsProvided()
        {
            // Act: Hacemos una petición GET a la API sin token de autenticación
            var response = await _client.GetAsync("/api/v1/Tasks");

            // Assert: Esperamos un código de estado 401 Unauthorized
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}