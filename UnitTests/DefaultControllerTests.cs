using System;
using Xunit;
using KOCR_Web.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using api.K_OCR;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using KOCR_Web.Models;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Http;
using Moq;
using api.K_OCR.Controllers;

// https://medium.com/imaginelearning/asp-net-core-3-1-microservice-quick-start-c0c2f4d6c7fa

namespace UnitTests {
    [Collection("Integration Tests")]
    public class DefaultControllerTests : IClassFixture<WebApplicationFactory<Startup>> {

        private readonly WebApplicationFactory<Startup> _factory;

        public DefaultControllerTests(WebApplicationFactory<Startup> factory) {
            _factory = factory;
        }

        [Fact]
        public async Task WeatherReport_Get_Should_Return_Correct_Count() {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/WeatherForecast");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(response.Content);

            string responseString = await response.Content.ReadAsStringAsync();
            WeatherForecast[] weatherForcast = System.Text.Json.JsonSerializer.Deserialize<WeatherForecast[]>(responseString);

            Assert.Equal(5, weatherForcast.Length);
        }

        [Fact]
        public void OCRController_Get_Should_Return_Values() {
            var options = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };
            var client = _factory.CreateClient(options);
            //var fileName = @"C:\\OCR\ex1.png";

            // Arrange.
            var fileMock = new Mock<IFormFile>();
            var physicalFile = new FileInfo("C:\\OCR\\ex1.png");
            FileStream fs = physicalFile.OpenRead();
            var ms = new MemoryStream();
            ms.SetLength(fs.Length);
            int bytesRead = fs.Read(ms.GetBuffer(), 0, (int)fs.Length);

            ms.Position = 0;

            //Setup mock file using info from physical file
            fileMock.Setup(_ => _.FileName).Returns(physicalFile.FullName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.ContentDisposition).Returns(string.Format("inline; filename={0}", physicalFile.FullName));


            // needs to find a way to mock IConfiguration settings, IOCRService ocrService
            // so you only need one constructor in OCRController

            //var sut = new OCRController();
            //var file = fileMock.Object;

            //// Act.
            //var result = await sut.Post(file);

            //Assert.
            //Assert.IsInstanceOfType(result, typeof(IActionResult));

        }
    }
}
