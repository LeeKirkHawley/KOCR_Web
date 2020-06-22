using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KOCR_Web.Models;
using KOCR_Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace KOCR_Web.Controllers {
    public class FileUploadController : Controller {

        private readonly IConfiguration _settings;
        private readonly OCRService _ocrService;

        public FileUploadController(IConfiguration settings, OCRService ocrService) {
            _settings = settings;
            _ocrService = ocrService;
        }

        [HttpPost]
        public IActionResult Upload(IFormFile[] files) {
            // Iterate through uploaded files array
            foreach (var file in files) {
                // Extract file name from whatever was posted by browser
                var originalFileName = System.IO.Path.GetFileName(file.FileName);

                var fileName = Guid.NewGuid().ToString();

                string imageFilePath = Path.Combine(_settings["ImageFilePath"], fileName);
                imageFilePath += Path.GetExtension(originalFileName);

                string textFilePath = Path.Combine(_settings["TextFilePath"], fileName);
                //textFilePath += ".txt;

                // If file with same name exists delete it
                if (System.IO.File.Exists(imageFilePath)) {
                    System.IO.File.Delete(imageFilePath);
                }

                // Create new local file and copy contents of uploaded file
                using (var localFile = System.IO.File.OpenWrite(imageFilePath))
                using (var uploadedFile = file.OpenReadStream()) {
                    uploadedFile.CopyTo(localFile);
                }

                //_ocrService.DoOCR(imageFilePath, fileName);
                _ocrService.DoOCR(imageFilePath, textFilePath);
            }

            ViewBag.Message = "Files successfully uploaded";

            return View();
        }       
    }
}
