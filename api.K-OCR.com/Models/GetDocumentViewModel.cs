using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace api.K_OCR.Models {
    public class GetDocumentViewModel {
        public string Msg { get; set; }
        public List<Document> Documents { get; set; }
    }
}
