using System;
using Xunit;
using Core.Services;
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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using KOCR_Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

// https://medium.com/imaginelearning/asp-net-core-3-1-microservice-quick-start-c0c2f4d6c7fa

namespace UnitTests {
    [Collection("Integration Tests")]
    public class DefaultControllerTests : IClassFixture<WebApplicationFactory<Startup>> {

        private readonly WebApplicationFactory<Startup> _factory;

        //private HttpContext _rmContext;
        //private HttpRequest _rmRequest;
        private Mock<HttpContext> _moqContext;
        private Mock<HttpRequest> _moqRequest;

        private class TestConfig {
            [Required]
            public string SomeKey { get; set; }
            [Required] //<--NOTE THIS
            public string SomeOtherKey { get; set; }
        }

        public static IConfiguration InitConfiguration() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }


        public DefaultControllerTests(WebApplicationFactory<Startup> factory) {
            _factory = factory;

            _moqContext = new Mock<HttpContext>();
            _moqRequest = new Mock<HttpRequest>();
            _moqContext.Setup(x => x.Request).Returns(_moqRequest.Object);
        }

        void Setup() {
        }

        [Fact]
        public async Task OCRController_Get_Should_Return_Values() {
            var options = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };
            var client = _factory.CreateClient(options);

            var configuration = InitConfiguration();

            // setup mock IWebHostEnvironment
            var webHostEnvironment = new Mock<IWebHostEnvironment>();

            // setup IOCRService
            var ocrService = new OCRService(configuration);

            // setup IHttpContextAccessor
            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            IndexViewModel model = new IndexViewModel {
                Language = "eng"
            };


            using (var stream = File.OpenRead("C:\\OCR\\ex2.JPG")) {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName("C:\\OCR\\ex2.JPG")) {
                    Headers = new HeaderDictionary(),
                    ContentType = "multipart/form-data"
                };

                IFormFile[] formFileArray = { file };

                var httpContext = new DefaultHttpContext();
                httpContext.Request.ContentType = "multipart/form-data";

                var controller = new HomeController(configuration, webHostEnvironment.Object, ocrService, httpContextAccessor.Object) {
                    ControllerContext = new ControllerContext {
                        HttpContext = httpContext
                    }
                };
                ActionResult response = await controller.Index(model, formFileArray);
                var viewResult = Assert.IsType<ViewResult>(response);
                // can not figure out a way to get real contents of the response
                // which means I can't get the status code
            }
        }
    }
}
