using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KOCR_Web.Services {
    public class OCRService {

        private readonly IConfiguration _settings;

        public OCRService(IConfiguration settings) {
            _settings = settings;
        }

        public void DoOCR(string imageName, string outputBase) {

            string TessPath = Path.Combine(_settings["TesseractPath"], "tesseract.exe");
           

            System.Diagnostics.Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = TessPath;
            p.StartInfo.ArgumentList.Add(imageName);
            p.StartInfo.ArgumentList.Add(outputBase);
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
        }
    }
}
