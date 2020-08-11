using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KOCR_Web.Models;
using Microsoft.AspNetCore.Http;
using NLog;
using System.Threading;
using System.IO;
using Microsoft.Extensions.Configuration;
using KOCR_Web.Services;
using Microsoft.AspNetCore.Hosting;

namespace KOCR_Web.Controllers {
    public class CWDocsController : Controller {

        private readonly Logger _debugLogger;
        private readonly IConfiguration _settings;
        private readonly OCRService _ocrService;
        private readonly IWebHostEnvironment _environment;


        public CWDocsController(IConfiguration settings, OCRService ocrService, IWebHostEnvironment environment) {
            _debugLogger = LogManager.GetLogger("debugLogger");
            _settings = settings;
            _ocrService = ocrService;
            _environment = environment;

            _ocrService.SetupLanguages();
        }


        public IActionResult Index() {
            CWDocsIndexViewModel model = new CWDocsIndexViewModel();

            return View(model);
        }

        [HttpPost]
        [RequestSizeLimit(1000000)]
        public async Task<ActionResult> Index(CWDocsIndexViewModel model, IFormFile[] files) {

            //_debugLogger.Debug($"Entering HomeController.Index()");
            DateTime startTime = DateTime.Now;

            string file = "";
            try {
                file = $"{files[0].FileName}";
            }
            catch (Exception ex) {
                _debugLogger.Debug(ex, "Exception reading file name.");
            }

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
                await _ocrService.OCRPDFFile(imageFilePath, textFilePath + ".tif", "eng");

            }
            else {
                errorMsg = await _ocrService.OCRImageFile(imageFilePath, textFilePath, "eng");
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


    }
}
