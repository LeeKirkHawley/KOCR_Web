using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace api.KOCR.Services {
    public interface IOCRService {
        public abstract Task<string> OCRImageFile(string imageName, string outputBase, string language);
        public abstract Task<string> OCRPDFFile(string pdfName, string outputFile, string language);
        public abstract List<SelectListItem> SetupLanguages();
        public abstract void ImmediateCleanup(string imageFilePath, string imageFileExtension, string textFilePath);
        public abstract void Cleanup(string imageFilePath, string textFilePath);
    }
}
