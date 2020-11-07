using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Packaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KOCR_Web.Models;
using Microsoft.AspNetCore.Http;
using KOCR_Web.Services;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using NLog;
using System.Threading;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics;

namespace KOCR_Web.Controllers {
    public class HomeController : Controller {
        private readonly IConfiguration _settings;
        private readonly IOCRService _ocrService;
        private readonly Logger _debugLogger;
        private readonly Logger _jobLogger;
        private readonly IWebHostEnvironment _environment;

        public HomeController(IConfiguration settings, IWebHostEnvironment environment, IOCRService ocrService) {
            _settings = settings;
            _ocrService = ocrService;
            _environment = environment;

            _jobLogger = LogManager.GetLogger("jobLogger");
            _debugLogger = LogManager.GetLogger("debugLogger");
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            string ExceptionMessage = "";

            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature?.Error is FileNotFoundException) {
                ExceptionMessage = "File error thrown";
            }
            if(exceptionHandlerPathFeature?.Error.Message == "Request body too large.") {
                ExceptionMessage = "File was too big - files are limited to 1000000 bytes.";
            }
            if (exceptionHandlerPathFeature?.Path == "/index") {
                ExceptionMessage += " from home page";
            }

            ErrorViewModel errorModel = new ErrorViewModel {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ExceptionMessage = ExceptionMessage
            };

            return View(errorModel);
        }

        [HttpGet]
        public IActionResult Index(IndexViewModel model) {
            if (model.OCRText == null) {
                model.OCRText = "";
            }
            model.Languages = _ocrService.SetupLanguages();
            model.Language = "eng";

            return View(model);
        }

        [HttpPost]
        [RequestSizeLimit(1000000)]
        public async Task<ActionResult> Index(IndexViewModel model, IFormFile[] files) {

            //if (files[0].Length > 1000000) {
            //    ModelState.AddModelError("File was too large.", new FileFormatException()); 
            //}
            _debugLogger.Debug($"Entering HomeController.Index()");
            DateTime startTime = DateTime.Now;

            string file = "";
            try {
                file = $"{files[0].FileName}";
            }
            catch (Exception ex) {
                _debugLogger.Debug(ex, "Exception reading file name.");
                throw;
            }

            _jobLogger.Info($"OCR file {file}");
            _debugLogger.Info($"Thread {Thread.CurrentThread.ManagedThreadId}: Processing file {file}");


            // Extract file name from whatever was posted by browser
            var originalFileName = System.IO.Path.GetFileName(files[0].FileName);
            string imageFileExtension = Path.GetExtension(originalFileName);

            var fileName = Guid.NewGuid().ToString();

            // set up the image file (input) path
            string imageFilePath = Path.Combine(_settings["ImageFilePath"], fileName);
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
            catch (Exception ex) {
                _debugLogger.Debug($"Couldn't write file {imageFilePath}");
                // HANDLE ERROR
            }

            string errorMsg = "";

            if (imageFileExtension.ToLower() == ".pdf") {
                await _ocrService.OCRPDFFile(imageFilePath, textFilePath + ".tif", model.Language);

            }
            else {
                errorMsg = await _ocrService.OCRImageFile(imageFilePath, textFilePath, model.Language);
            }

            string textFileName = textFilePath + ".txt";
            string ocrText = "";
            try {
                ocrText = System.IO.File.ReadAllText(textFileName);
            }
            catch (Exception ex) {
                _debugLogger.Debug($"Couldn't read text file {textFileName}");
            }

            if (ocrText == "") {
                if (errorMsg == "")
                    ocrText = "No text found.";
                else
                    ocrText = errorMsg;
            }

            // update model for display of ocr'ed data
            model.OCRText = ocrText;
            model.OriginalFileName = originalFileName;
            model.CacheFilename = Path.GetFileName(textFileName);
            model.Languages = _ocrService.SetupLanguages();

            DateTime finishTime = DateTime.Now;
            TimeSpan ts = (finishTime - startTime);
            string duration = ts.ToString(@"hh\:mm\:ss");

            _ocrService.Cleanup(_settings["ImageFilePath"], _settings["TextFilePath"]);

            _debugLogger.Info($"Thread {Thread.CurrentThread.ManagedThreadId}: Finished processing file {file} Elapsed time: {duration}");
            //_debugLogger.Debug($"Leaving HomeController.Index()");

            return View(model);
        }

        [HttpGet]
        public IActionResult AboutMe() {
            AboutMeViewModel model = new AboutMeViewModel();
            return View(model);
        }

        [HttpGet]
            public IActionResult DownloadOCR(string cacheFileName, string originalFileName) {
            _debugLogger.Debug($"Entering HomeController.DownloadOCR()");

            string textFileName = Path.GetFileNameWithoutExtension(originalFileName) + ".txt";
            _debugLogger.Debug($"textFileName: {textFileName}");

            //var path = Path.Combine(_environment.WebRootPath, Path.Combine(Path.Combine(_settings["TextFilePath"], cacheFileName)));
            string path = Path.Combine(_settings["TextFilePath"], cacheFileName);

            FileStream fs = null;
            try {
                fs = new FileStream(path, FileMode.Open);
            }
            catch(Exception ex) {
                _debugLogger.Debug(ex, $"Error opening {path}");
            }

            _debugLogger.Debug($"Leaving HomeController.DownloadOCR()");

            return File(fs, "application/octet-stream", textFileName);
        }
    }
}
