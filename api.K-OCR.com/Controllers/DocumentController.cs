using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.K_OCR.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase {
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger) {
            _logger = logger;
        }

        //public IActionResult Get() {
        //    return View();
        //}
    }
}
