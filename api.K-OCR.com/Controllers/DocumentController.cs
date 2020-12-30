using api.K_OCR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Services;
using Core.Models;
using Microsoft.AspNetCore.Cors;
using Newtonsoft;
using Newtonsoft.Json;
using Core.Extensions;

namespace api.K_OCR.Controllers {
    [ApiController]
    public class DocumentController : Controller {
        private readonly ILogger<DocumentController> _logger;
        private readonly SQLiteDBContext _context;

        public DocumentController(ILogger<DocumentController> logger, SQLiteDBContext context) {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("Document")]
        //[DisableCors()]
        public IActionResult Get() {

            List<Document> docList = _context.Documents.ToList();

            //Returning Json Data  
            var json = Json(new { data = docList });
            return json;
        }
    }
}
