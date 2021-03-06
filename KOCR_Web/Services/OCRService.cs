﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KOCR_Web.Extensions;
using NLog.Web;
using NLog;
using Microsoft.Extensions.Logging;

namespace KOCR_Web.Services {
    public class OCRService {

        private readonly IConfiguration _settings;
        private readonly Logger _debugLogger;

        public OCRService(IConfiguration settings) {
            _settings = settings;

            _debugLogger = LogManager.GetLogger("debugLogger");
        }

        public async void OCRImageFile(string imageName, string outputBase) {

            //_debugLogger.Debug("Entering OCRImageFile()");

            string TessPath = Path.Combine(_settings["TesseractPath"], "tesseract.exe");
           

            System.Diagnostics.Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = TessPath;
            p.StartInfo.ArgumentList.Add(imageName);
            p.StartInfo.ArgumentList.Add(outputBase);

            try {
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                await p.WaitForExitAsync(1000000);
            }
            catch(Exception ex) {
                _debugLogger.Debug(ex, "Couldn't run Tesseract");
            }
        }

        public async Task OCRPDFFile(string pdfName, string outputFile) {

            string outputBase = _settings["TextFilePath"] + "\\" + Path.GetFileNameWithoutExtension(pdfName);
            string tifFileName = outputBase + ".tif";

            // convert pdf to tif
            using (System.Diagnostics.Process p = new Process()) {
                string GhostscriptPath = Path.Combine(_settings["GhostscriptPath"], "gswin64c.exe");  // 'c' version doesn't show ui window

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

        }
    }
}
