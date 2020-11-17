using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.K_OCR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NLog;
using api.KOCR.Services;
using System.Threading;
using System.IO;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.K_OCR.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class OCRController : ControllerBase {
        private readonly IConfiguration _settings;
        private readonly IOCRService _ocrService;
        private readonly Logger _debugLogger;
        private readonly Logger _jobLogger;

        public OCRController(IConfiguration settings, IOCRService ocrService) {
            _settings = settings;
            _ocrService = ocrService;
        }

        // GET: api/<OCRController>
        [HttpGet]
        public IEnumerable<string> Get() {
            return new string[] { "value1", "value2" };
        }

        // GET api/<OCRController>/5
        [HttpGet("{id}")]
        public string Get(int id) {
            return "value";
        }

        // POST api/<OCRController>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(1000000)]
        public async Task<OCRModel> Post([FromBody] OCRModel model, IFormFile[] files) {
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

            //OCRModel model = new OCRModel();

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

            return model;

        }

        // PUT api/<OCRController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/<OCRController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}
