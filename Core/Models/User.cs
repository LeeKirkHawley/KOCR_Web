using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models {
    public class User {
        [Key]
        public int Id { get; set; }
        public string userName { get; set; }
        public string pwd { get; set; }
        public string role { get; set; }
    }
}
