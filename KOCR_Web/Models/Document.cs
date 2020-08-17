using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KOCR_Web.Models {
    public class Document {
        [Key]
        [Column("Id")]
        public int fileId { get; set; }
        public int userId { get; set; }
        public string originalDocumentName { get; set; }
        public string documentName { get; set; }

    }
}
