using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KOCR_Web.Models;
using Microsoft.AspNetCore.Http;
using KOCR_Web.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using NLog.Web;
using NLog;


namespace KOCR_Web.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _settings;
        private readonly OCRService _ocrService;

        public HomeController(ILogger<HomeController> logger, IConfiguration settings, OCRService ocrService) {
            _logger = logger;
            _settings = settings;
            _ocrService = ocrService;
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Index(IndexViewModel model) {
            if (model.OCRText == null) {
                model.OCRText = "";
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(IFormFile[] files) {

            //_logger.LogInformation($"Processing {files[0].FileName}");

            Logger jobLogger = LogManager.GetLogger("jobs");
            jobLogger.Info($"OCR file {files[0].FileName}");



            // Extract file name from whatever was posted by browser
            var originalFileName = System.IO.Path.GetFileName(files[0].FileName);

            var fileName = Guid.NewGuid().ToString();

            // set up the image file (input) path
            string imageFilePath = Path.Combine(_settings["ImageFilePath"], fileName);
            string imageFileExtension = Path.GetExtension(originalFileName);
            imageFilePath += imageFileExtension;

            // set up the text file (output) path
            string textFilePath = Path.Combine(_settings["TextFilePath"], fileName);

            // If file with same name exists delete it
            if (System.IO.File.Exists(imageFilePath)) {
                System.IO.File.Delete(imageFilePath);
            }

            // Create new local file and copy contents of uploaded file
            using (var localFile = System.IO.File.OpenWrite(imageFilePath))
            using (var uploadedFile = files[0].OpenReadStream()) {
                uploadedFile.CopyTo(localFile);
            }

            if (imageFileExtension.ToLower() == ".pdf") {
                await _ocrService.OCRPDFFile(imageFilePath, textFilePath + ".tif");
                
            }
            else {
                _ocrService.OCRImageFile(imageFilePath, textFilePath);
            }

            string ocrText = System.IO.File.ReadAllText(textFilePath + ".txt");

            System.IO.File.Delete(imageFilePath);
            System.IO.File.Delete(textFilePath + ".txt");

            IndexViewModel model = new IndexViewModel {
                OCRText = ocrText,
                originalFileName = originalFileName
            };

//            ViewBag.Message = "Files successfully uploaded";

            return View(model);
        }

    }
}
