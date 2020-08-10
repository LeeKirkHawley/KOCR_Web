using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KOCR_Web.Models;

namespace KOCR_Web.Controllers {
    public class CWDocsController : Controller {
        public IActionResult Index() {
            CWDocsIndexViewModel model = new CWDocsIndexViewModel();

            return View(model);
        }
    }
}
