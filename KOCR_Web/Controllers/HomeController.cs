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
using System.IO;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using NLog;
using System.Threading;

namespace KOCR_Web.Controllers {
    public class HomeController : Controller {
        private readonly IConfiguration _settings;
        private readonly OCRService _ocrService;
        private readonly Logger _debugLogger;
        private readonly Logger _jobLogger;

        public HomeController(IConfiguration settings, OCRService ocrService) {
            _settings = settings;
            _ocrService = ocrService;

            _jobLogger = LogManager.GetLogger("jobLogger");
            _debugLogger = LogManager.GetLogger("debugLogger");
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

            //_debugLogger.Debug($"Entering HomeController.Index()");
            DateTime startTime = DateTime.Now;

            string file = "";
            try {
                file = $"{files[0].FileName}";
            }
            catch (Exception ex) {
                _debugLogger.Debug(ex, "Exception reading file name.");
            }

            _jobLogger.Info($"OCR file {file}");
            _debugLogger.Info($"Thread {Thread.CurrentThread.ManagedThreadId}: Processing file {file}");

            // Extract file name from whatever was posted by browser
            var originalFileName = System.IO.Path.GetFileName(files[0].FileName);

            var fileName = Guid.NewGuid().ToString();

            // set up the image file (input) path
            string imageFilePath = Path.Combine(_settings["ImageFilePath"], fileName);
            string imageFileExtension = Path.GetExtension(originalFileName);
            imageFilePath += imageFileExtension;

            //_debugLogger.Info($"ImageFilePath: {imageFilePath}");
            //_debugLogger.Info($"Current: {Directory.GetCurrentDirectory()}");

            // set up the text file (output) path
            string textFilePath = Path.Combine(_settings["TextFilePath"], fileName);

            // If file with same name exists delete it
            if (System.IO.File.Exists(imageFilePath)) {
                System.IO.File.Delete(imageFilePath);
            }

            // Create new local file and copy contents of uploaded file
            try {
                using (var localFile = System.IO.File.OpenWrite(imageFilePath))
                using (var uploadedFile = files[0].OpenReadStream()) {
                    uploadedFile.CopyTo(localFile);
                }
            }
            catch(Exception ex) {
                _debugLogger.Debug($"Couldn't write file {imageFilePath}");
                // HANDLE ERROR
            }

            if (imageFileExtension.ToLower() == ".pdf") {
                await _ocrService.OCRPDFFile(imageFilePath, textFilePath + ".tif");
                
            }
            else {
                _ocrService.OCRImageFile(imageFilePath, textFilePath);
            }

            string textFileName = textFilePath + ".txt";
            string ocrText = "";
            try {
                ocrText = System.IO.File.ReadAllText(textFileName);
            }
            catch(Exception ex) {
                _debugLogger.Debug($"Couldn't read text file {textFileName}");
            }

            try {
                System.IO.File.Delete(imageFilePath);
                System.IO.File.Delete(textFilePath + ".txt");
            }
            catch(Exception ex) {
                _debugLogger.Debug("Failed to delete OCR files.");
                // HANDLE ERROR
            }

            IndexViewModel model = new IndexViewModel {
                OCRText = ocrText,
                originalFileName = originalFileName
            };

            DateTime finishTime = DateTime.Now;
            TimeSpan ts = (finishTime - startTime);
            string duration = ts.ToString(@"hh\:mm\:ss");

            _debugLogger.Info($"Thread {Thread.CurrentThread.ManagedThreadId}: Finished processing file {file}: {duration}");
            //_debugLogger.Debug($"Leaving HomeController.Index()");

            return View(model);
        }
    }
}
