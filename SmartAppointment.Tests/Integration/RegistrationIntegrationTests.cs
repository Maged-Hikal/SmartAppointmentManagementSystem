using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartAppointment.Tests.Integration
{
    public class RegistrationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public RegistrationIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Configure test services if needed
                });
            });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_Endpoint_With_Valid_Data_Should_Return_Success()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "Integration Test User",
                Email = "integration.test@example.com",
                PhoneNumber = "+1234567890",
                SSN = "555555555555",
                Password = "IntegrationTest123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/me/register", model);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Register_Endpoint_With_Duplicate_SSN_Should_Return_BadRequest()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "First User",
                Email = "first.user@example.com",
                PhoneNumber = "+1234567890",
                SSN = "666666666666",
                Password = "Password123"
            };

            // First registration
            await _client.PostAsJsonAsync("/api/me/register", model);

            // Second registration with same SSN
            var duplicateModel = new RegisterModel
            {
                UserName = "Second User",
                Email = "second.user@example.com",
                PhoneNumber = "+0987654321",
                SSN = "666666666666", // Same SSN
                Password = "Password123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/me/register", duplicateModel);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorMessages = JsonSerializer.Deserialize<string[]>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            errorMessages.Should().ContainSingle();
            errorMessages[0].Should().Be("SSN already registered, please choose another SSN");
        }

        [Fact]
        public async Task Register_Endpoint_With_Invalid_Data_Should_Return_BadRequest()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "", // Invalid: empty
                Email = "invalid-email", // Invalid: not a valid email
                PhoneNumber = "invalid-phone", // Invalid: not a valid phone
                SSN = "123", // Invalid: too short
                Password = "123" // Invalid: too short
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/me/register", model);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_Endpoint_With_Minimal_Data_Should_Return_Success()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "Minimal User",
                Email = null, // Optional field
                PhoneNumber = null, // Optional field
                SSN = "777777777777",
                Password = "MinimalUser123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/me/register", model);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Register_Endpoint_With_Special_Characters_In_UserName_Should_Return_Success()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "José María González-O'Reilly",
                Email = "jose.maria@example.com",
                PhoneNumber = "+1234567890",
                SSN = "888888888888",
                Password = "SpecialChars123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/me/register", model);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Register_Endpoint_Concurrent_Requests_Should_Handle_Duplicates()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = "Concurrent User",
                Email = "concurrent@example.com",
                PhoneNumber = "+1234567890",
                SSN = "999999999999",
                Password = "ConcurrentUser123"
            };

            // Act - Create multiple concurrent requests
            var tasks = new List<Task<HttpResponseMessage>>();
            for (int i = 0; i < 3; i++)
            {
                var concurrentModel = new RegisterModel
                {
                    UserName = $"Concurrent User {i}",
                    Email = $"concurrent{i}@example.com",
                    PhoneNumber = $"+123456789{i}",
                    SSN = "999999999999", // Same SSN for all
                    Password = "ConcurrentUser123"
                };
                tasks.Add(_client.PostAsJsonAsync("/api/me/register", concurrentModel));
            }

            var responses = await Task.WhenAll(tasks);

            // Assert
            var successCount = 0;
            var badRequestCount = 0;

            foreach (var response in responses)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    successCount++;
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    badRequestCount++;
            }

            successCount.Should().Be(1); // Only one should succeed
            badRequestCount.Should().Be(2); // Two should fail due to duplicate SSN
        }

        [Fact]
        public async Task Register_Endpoint_With_Maximum_Length_Values_Should_Return_Success()
        {
            // Arrange
            var model = new RegisterModel
            {
                UserName = new string('A', 50), // Maximum allowed length
                Email = "very.long.email.address@example.com",
                PhoneNumber = "+12345678901234567890",
                SSN = "111111111111",
                Password = new string('A', 100) // Maximum allowed length
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/me/register", model);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
