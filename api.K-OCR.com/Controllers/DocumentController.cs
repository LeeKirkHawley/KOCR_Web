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

namespace api.K_OCR.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase {
        private readonly ILogger<DocumentController> _logger;
        private readonly SQLiteDBContext _context;

        public DocumentController(ILogger<DocumentController> logger, SQLiteDBContext context) {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public GetDocumentViewModel Get() {
            GetDocumentViewModel model = new GetDocumentViewModel {
                Msg = "Here's the message.",
                Documents = _context.Documents.ToList<Document>()
            };

            return model;
        }
    }
}
