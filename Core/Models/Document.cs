using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models {
    public record Document {
        [Key]
        [Column("Id")]
        public int fileId { get; init; }
        public int userId { get; init; }
        public string originalDocumentName { get; init; }
        public string documentName { get; init; }

    }
}
