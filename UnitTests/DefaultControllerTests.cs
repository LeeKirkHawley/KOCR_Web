using System;
using Xunit;
using KOCR_Web.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using api.K_OCR;
using System.Threading.Tasks;
using System.Text.Json;

// https://medium.com/imaginelearning/asp-net-core-3-1-microservice-quick-start-c0c2f4d6c7fa

namespace UnitTests {
    [Collection("Integration Tests")]
    public class DefaultControllerTests : IClassFixture<WebApplicationFactory<Startup>> {

        private readonly WebApplicationFactory<Startup> _factory;

        public DefaultControllerTests(WebApplicationFactory<Startup> factory) {
            _factory = factory;
        }

        [Fact]
        public async Task WeatherReport_Get_Should_Return_Correct_Countp() {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/WeatherForecast");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(response.Content);

            string responseString = await response.Content.ReadAsStringAsync();
            WeatherForecast[] weatherForcast = System.Text.Json.JsonSerializer.Deserialize<WeatherForecast[]>(responseString);

            Assert.Equal(5, weatherForcast.Length);
        }

        private class ResponseType {
            public string Status { get; set; }
        }
    }
}
