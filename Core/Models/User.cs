using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models {
    public record User {
        [Key]
        public int Id { get; init; }
        public string userName { get; init; }
        public string pwd { get; init; }
        public string role { get; init; }
    }
}
