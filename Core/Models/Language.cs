using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models {
    public record Language {
        public string Text { get; init; }
        public string Value { get; init; }
    }
}
