using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KOCR_Web.Extensions;

namespace KOCR_Web.Services {
    public class OCRService {

        private readonly IConfiguration _settings;

        public OCRService(IConfiguration settings) {
            _settings = settings;
        }

        public void OCRImageFile(string imageName, string outputBase) {

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

        public async Task OCRPDFFile(string pdfName, string outputFile) {

            string outputBase = _settings["TextFilePath"] + "\\" + Path.GetFileNameWithoutExtension(pdfName);
            string tifFileName = outputBase + ".tif";

            // convert pdf to tif
            using (System.Diagnostics.Process p = new Process()) {
                string GhostscriptPath = Path.Combine(_settings["GhostscriptPath"], "gswin64.exe");

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = GhostscriptPath;
                p.StartInfo.ArgumentList.Add("-dNOPAUSE");
                p.StartInfo.ArgumentList.Add("-r300");
                p.StartInfo.ArgumentList.Add("-sDEVICE=tiffscaled24");
                p.StartInfo.ArgumentList.Add("-sCompression=lzw");
                p.StartInfo.ArgumentList.Add("-dBATCH");
                p.StartInfo.ArgumentList.Add($"-sOutputFile={tifFileName}");
                p.StartInfo.ArgumentList.Add(pdfName);
                bool result = p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                await p.WaitForExitAsync(1000000);
            }

            OCRImageFile(tifFileName, outputBase);

            // ocr resulting tif
            //using (System.Diagnostics.Process p = new Process()) {
            //    string ImageMagickPath = Path.Combine(_settings["ImageMagickPath"], "convert.exe");

            //    p.StartInfo.UseShellExecute = false;
            //    p.StartInfo.RedirectStandardOutput = true;
            //    p.StartInfo.FileName = ImageMagickPath;
            //    //p.StartInfo.ArgumentList.Add("-density");
            //    //p.StartInfo.ArgumentList.Add("600");
            //    p.StartInfo.ArgumentList.Add(pdfName);
            //    p.StartInfo.ArgumentList.Add(outputFile);
            //    bool result = p.Start();
            //    // Do not wait for the child process to exit before
            //    // reading to the end of its redirected stream.
            //    // p.WaitForExit();
            //    // Read the output stream first and then wait.
            //    string output = p.StandardOutput.ReadToEnd();
            //    p.WaitForExit();
            //}
        }

    }
}
