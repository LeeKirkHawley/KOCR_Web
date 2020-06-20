using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KOCR_Web.Services {
    public class OCRService {
        public void DoOCR(string imageName, string outputBase) {
            System.Diagnostics.Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "c:\\\\Program Files\\Tesseract-OCR\\tesseract";
            //p.StartInfo.Arguments= "C:\\\\Work\\OCR Files\\phototest.tif C:\\\\Work\\OCR Files\\phototest";
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
