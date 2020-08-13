using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KOCR_Web.Models {
    public class Document {
        [Key]
        public int Id { get; set; }
        public int userId { get; set; }
        public string documentName { get; set; }
        public string originalDocumentName { get; set; }

    }
}
