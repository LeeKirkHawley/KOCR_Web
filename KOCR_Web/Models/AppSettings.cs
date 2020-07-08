using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KOCR_Web.Models {
    public class AppSettings {
        string TesseractPath { get; set; }
        string ImageMagickPath { get; set; }
        string GhostscriptPath { get; set; }
        string ImageFilePath { get; set; }
        string TextFilePath { get; set; }
        string MaxPDFSize { get; set; }
        string MaxTextSize { get; set; }
    }
}
