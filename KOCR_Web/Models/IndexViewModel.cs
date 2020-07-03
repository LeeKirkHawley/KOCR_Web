using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KOCR_Web.Models {
    public class IndexViewModel {
        public string OCRText { get; set; }
        public string OriginalFileName { get; set; }

        public string Language { get; set; }
        public List<SelectListItem> Languages { get; set; }
    }
}
