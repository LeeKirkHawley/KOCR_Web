using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KOCR_Web.Models;
using System.Diagnostics;
using KOCR_Web.Services;

namespace KOCR_Web.Controllers {
    public class OCRController : Controller {

        private readonly OCRService _ocrService;

        public OCRController(OCRService ocrService) {
            _ocrService = ocrService;
        }

        public IActionResult OCR() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoOCR([FromForm] OCRViewModel model) {

            _ocrService.DoOCR("C:\\\\Work\\OCR Files\\phototest.tif", "C:\\\\Work\\OCR Files\\phototest");

            return Content("OCR Text");
        }
    }
}
