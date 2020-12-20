using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using api.K_OCR;
using api.K_OCR.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft;
using Newtonsoft.Json;

namespace UnitTests {
    [Collection("WebAPI Tests")]
    public class APITests : IClassFixture<WebApplicationFactory<Startup>> {

        private readonly WebApplicationFactory<Startup> _factory;

        public APITests(WebApplicationFactory<Startup> factory) {
            _factory = factory;
        }

        [Fact]
        public async Task API_Get_Should_Get_Documents() {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Document");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(response.Content);

            string responseString = await response.Content.ReadAsStringAsync();
            GetDocumentViewModel docs = JsonConvert.DeserializeObject<GetDocumentViewModel>(responseString);

            Assert.Equal("Here's the message.", docs.Msg);
        }
    }
}
