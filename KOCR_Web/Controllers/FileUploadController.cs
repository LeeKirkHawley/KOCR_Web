using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KOCR_Web.Models;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Core;

namespace KOCR_Web.Controllers {
    public class FileUploadController : Controller {

        private readonly IConfiguration _settings;
        private readonly IOCRService _ocrService;

        public FileUploadController(IConfiguration settings, IOCRService ocrService) {
            _settings = settings;
            _ocrService = ocrService;
        }

    }
}
