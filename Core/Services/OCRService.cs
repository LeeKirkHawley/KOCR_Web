using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;
using NLog.Web;
using NLog;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Core.Models;
using System.Runtime.InteropServices.ComTypes;
using Core.Services;

namespace Core.Services {
    public class OCRService : IOCRService {

        private readonly IConfiguration _settings;
        private readonly Logger _debugLogger;

        public OCRService(IConfiguration settings) {
            _settings = settings;

            _debugLogger = LogManager.GetLogger("debugLogger");
        }

        public async Task<string> OCRImageFile(string imageName, string outputBase, string language) {

            //_debugLogger.Debug("Entering OCRImageFile()");

            string TessPath = Path.Combine(_settings["TesseractPath"], "tesseract.exe");
           

            System.Diagnostics.Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = TessPath;

            p.StartInfo.ArgumentList.Add(imageName);
            p.StartInfo.ArgumentList.Add(outputBase);
            p.StartInfo.ArgumentList.Add("-l");
            p.StartInfo.ArgumentList.Add(language);


            string returnMsg = "";
            try {
                p.Start();
                string stdOutput = p.StandardOutput.ReadToEnd();
                string errOutput = p.StandardError.ReadToEnd();
                if(errOutput != "") {
                    if(errOutput.Contains("Failed loading language")) {  // missing language file
                        _debugLogger.Debug(errOutput);
                        returnMsg =  "ERROR: Couldn't load language file " + language;
                    }
                    else {
                        // there will be another error string, probably with a msg saying dpi wasn't found, etc.
                        // this actually seems to be a success msg
                    }
                }
                await p.WaitForExitAsync(1000000);
            }
            catch(Exception ex) {
                _debugLogger.Debug(ex, "Couldn't run Tesseract");
            }

            return returnMsg;
        }

        public async Task<string> OCRPDFFile(string pdfName, string outputFile, string language) {

            string outputBase = _settings["TextFilePath"] + "\\" + Path.GetFileNameWithoutExtension(pdfName);
            string tifFileName = outputBase + ".tif";

            // convert pdf to tif
            using (System.Diagnostics.Process p = new Process()) {
                string GhostscriptPath = Path.Combine(_settings["GhostscriptPath"], "gswin64c.exe");  // "gswin64 'c' version doesn't show ui window

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = GhostscriptPath;
                p.StartInfo.ArgumentList.Add("-dNOPAUSE");
                p.StartInfo.ArgumentList.Add("-r300");
                p.StartInfo.ArgumentList.Add("-sDEVICE=tiffscaled24");
                //p.StartInfo.ArgumentList.Add("-sDEVICE=tiffg4");
                p.StartInfo.ArgumentList.Add("-sCompression=lzw");
                p.StartInfo.ArgumentList.Add("-dBATCH");
                p.StartInfo.ArgumentList.Add($"-sOutputFile={tifFileName}");
                p.StartInfo.ArgumentList.Add(pdfName);
                bool result = p.Start();
                string output = p.StandardOutput.ReadToEnd();
                await p.WaitForExitAsync(1000000);
            }

            return await OCRImageFile(tifFileName, outputBase, language);

        }

        public List<SelectListItem> SetupLanguages() {

            List<SelectListItem> languageItems = new List<SelectListItem>();

            if (File.Exists(@"Languages.json")) {
                String JSONtxt = File.ReadAllText(@"Languages.json");

                List<Language> languages = null;
                languages = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Language>>(JSONtxt).ToList<Language>();

                IOrderedEnumerable<Language> lan = from element in languages
                              orderby element.Text
                              select element;

                foreach (Language language in lan) {
                    languageItems.Add(new SelectListItem { Text = language.Text, Value = language.Value });
                }
            }

            return languageItems;


            ////languageItems.Add(new SelectListItem { Text = "Afrikaans", Value = "afr" });
            ////languageItems.Add(new SelectListItem { Text = "Amharic", Value = "ara" });
            //languageItems.Add(new SelectListItem { Text = "Arabic", Value = "ara" });
            ////languageItems.Add(new SelectListItem { Text = "Assamese", Value = "asm" });
            ////languageItems.Add(new SelectListItem { Text = "Azerbaijani", Value = "aze" });
            ////languageItems.Add(new SelectListItem { Text = "Azerbaijani - Cyrilic", Value = "aze_cyrl" });
            ////languageItems.Add(new SelectListItem { Text = "Belarusian", Value = "bel" });
            ////languageItems.Add(new SelectListItem { Text = "Bengali", Value = "ben" });
            ////languageItems.Add(new SelectListItem { Text = "Bosnian", Value = "bos" });
            //languageItems.Add(new SelectListItem { Text = "Bulgarian", Value = "bul" });
            ////languageItems.Add(new SelectListItem { Text = "Catalan; Valencian", Value = "cat" });
            ////languageItems.Add(new SelectListItem { Text = "Cebuano", Value = "ceb" });
            //languageItems.Add(new SelectListItem { Text = "Czech", Value = "ces" });
            //languageItems.Add(new SelectListItem { Text = "Chinese - Simplified", Value = "chi_sim" });
            //languageItems.Add(new SelectListItem { Text = "Chinese - Traditional", Value = "chi_tra" });
            ////languageItems.Add(new SelectListItem { Text = "Cherokee", Value = "chr" });
            //languageItems.Add(new SelectListItem { Text = "Danish", Value = "dan" });
            ////languageItems.Add(new SelectListItem { Text = "Danish - Fraktur", Value = "dan_frak" });
            //languageItems.Add(new SelectListItem { Text = "German", Value = "deu" });
            ////languageItems.Add(new SelectListItem { Text = "German - Fraktur", Value = "deu_frak" });
            ////languageItems.Add(new SelectListItem { Text = "Dzongkha", Value = "dzo" });
            //languageItems.Add(new SelectListItem { Text = "Greek, Modern(1453 -)", Value = "ell" });
            //languageItems.Add(new SelectListItem { Text = "English", Value = "eng" });
            ////languageItems.Add(new SelectListItem { Text = "Middle(1100 - 1500)", Value = "enm" });
            ////languageItems.Add(new SelectListItem { Text = "Esperanto", Value = "cym" });
            ////languageItems.Add(new SelectListItem { Text = "Math / equation detection module", Value = "equ" });
            ////languageItems.Add(new SelectListItem { Text = "Basque", Value = "eus" });
            ////languageItems.Add(new SelectListItem { Text = "Persian", Value = "fas" });
            //languageItems.Add(new SelectListItem { Text = "Finnish", Value = "fin" });
            //languageItems.Add(new SelectListItem { Text = "French", Value = "fra" });
            ////languageItems.Add(new SelectListItem { Text = "Frankish", Value = "frk" });
            ////languageItems.Add(new SelectListItem { Text = "French, Middle(ca.1400 - 1600)", Value = "frm" });
            ////languageItems.Add(new SelectListItem { Text = "Galician", Value = "glg" });
            //languageItems.Add(new SelectListItem { Text = "Greek, Ancient(to 1453)", Value = "grc" });
            ////languageItems.Add(new SelectListItem { Text = "Gujarati", Value = "guj" });
            ////languageItems.Add(new SelectListItem { Text = "Haitian; Haitian Creole", Value = "hat" });
            //languageItems.Add(new SelectListItem { Text = "Hebrew", Value = "heb" });
            //languageItems.Add(new SelectListItem { Text = "Croatian", Value = "hrv" });
            //languageItems.Add(new SelectListItem { Text = "Hungarian", Value = "hun" });
            ////languageItems.Add(new SelectListItem { Text = "Inuktitut", Value = "iku" });
            //languageItems.Add(new SelectListItem { Text = "Indonesian", Value = "ind" });
            //languageItems.Add(new SelectListItem { Text = "Icelandic", Value = "isl" });
            //languageItems.Add(new SelectListItem { Text = "Italian", Value = "ita" });
            ////languageItems.Add(new SelectListItem { Text = "Italian - Old", Value = "ita_old" });
            ////languageItems.Add(new SelectListItem { Text = "Javanese", Value = "jav" });
            //languageItems.Add(new SelectListItem { Text = "Japanese", Value = "jpn" });
            ////languageItems.Add(new SelectListItem { Text = "Kannada", Value = "kan" });
            ////languageItems.Add(new SelectListItem { Text = "Georgian", Value = "kat" });
            ////languageItems.Add(new SelectListItem { Text = "Georgian - Old", Value = "kat_old" });
            ////languageItems.Add(new SelectListItem { Text = "Kazakh", Value = "kaz" });
            ////languageItems.Add(new SelectListItem { Text = "Central Khmer", Value = "khm" });
            ////languageItems.Add(new SelectListItem { Text = "Kirghiz; Kyrgyz", Value = "kir" });
            //languageItems.Add(new SelectListItem { Text = "Korean", Value = "kor" });
            ////languageItems.Add(new SelectListItem { Text = "Kurdish", Value = "kur" });
            ////languageItems.Add(new SelectListItem { Text = "Lao", Value = "lao" });
            //languageItems.Add(new SelectListItem { Text = "Latin", Value = "lat" });
            //languageItems.Add(new SelectListItem { Text = "Latvian", Value = "lav" });
            //languageItems.Add(new SelectListItem { Text = "Lithuanian", Value = "lit" });
            ////languageItems.Add(new SelectListItem { Text = "Malayalam", Value = "mal" });
            ////languageItems.Add(new SelectListItem { Text = "Marathi", Value = "mar" });
            ////languageItems.Add(new SelectListItem { Text = "Macedonian", Value = "mkd" });
            ////languageItems.Add(new SelectListItem { Text = "Maltese", Value = "mlt" });
            ////languageItems.Add(new SelectListItem { Text = "Malay", Value = "msa" });
            ////languageItems.Add(new SelectListItem { Text = "Burmese", Value = "mya" });
            ////languageItems.Add(new SelectListItem { Text = "Nepali", Value = "nep" });
            //languageItems.Add(new SelectListItem { Text = "Dutch; Flemish", Value = "nld" });
            //languageItems.Add(new SelectListItem { Text = "Norwegian", Value = "nor" });
            ////languageItems.Add(new SelectListItem { Text = "Oriya", Value = "ori" });
            ////languageItems.Add(new SelectListItem { Text = "Orientation and script detection module", Value = "osd" });
            ////languageItems.Add(new SelectListItem { Text = "Panjabi; Punjabi", Value = "pan" });
            //languageItems.Add(new SelectListItem { Text = "Polish", Value = "pol" });
            //languageItems.Add(new SelectListItem { Text = "Portuguese", Value = "por" });
            ////languageItems.Add(new SelectListItem { Text = "Pushto; Pashto", Value = "pus" });
            ////languageItems.Add(new SelectListItem { Text = "Romanian; Moldavian; Moldovan", Value = "ron" });
            //languageItems.Add(new SelectListItem { Text = "Russian", Value = "rus" });
            ////languageItems.Add(new SelectListItem { Text = "Sanskrit", Value = "san" });
            ////languageItems.Add(new SelectListItem { Text = "Sinhala; Sinhalese", Value = "sin" });
            ////languageItems.Add(new SelectListItem { Text = "Slovak", Value = "slk" });
            ////languageItems.Add(new SelectListItem { Text = "Slovak - Fraktur", Value = "slk_frak" });
            ////languageItems.Add(new SelectListItem { Text = "Slovenian", Value = "slv" });
            //languageItems.Add(new SelectListItem { Text = "Spanish; Castilian", Value = "spa" });
            ////languageItems.Add(new SelectListItem { Text = "Spanish; Castilian - Old", Value = "spa_old" });
            ////languageItems.Add(new SelectListItem { Text = "Serbian", Value = "srp" });
            ////languageItems.Add(new SelectListItem { Text = "Serbian - Latin", Value = "srp_latn" });
            ////languageItems.Add(new SelectListItem { Text = "Swahili", Value = "swa" });
            //languageItems.Add(new SelectListItem { Text = "Swedish", Value = "swe" });
            ////languageItems.Add(new SelectListItem { Text = "Syriac", Value = "syr" });
            //languageItems.Add(new SelectListItem { Text = "Tamil", Value = "tam" });
            ////languageItems.Add(new SelectListItem { Text = "Telugu", Value = "tel" });
            ////languageItems.Add(new SelectListItem { Text = "Tajik", Value = "tgk" });
            //languageItems.Add(new SelectListItem { Text = "Tagalog", Value = "tgl" });
            //languageItems.Add(new SelectListItem { Text = "Thai", Value = "tha" });
            ////languageItems.Add(new SelectListItem { Text = "Tigrinya", Value = "tir" });
            //languageItems.Add(new SelectListItem { Text = "Turkish", Value = "tur" });
            //languageItems.Add(new SelectListItem { Text = "Uighur; Uyghur", Value = "uig" });
            ////languageItems.Add(new SelectListItem { Text = "Ukrainian", Value = "ukr" });
            ////languageItems.Add(new SelectListItem { Text = "Urdu", Value = "urd" });
            ////languageItems.Add(new SelectListItem { Text = "Uzbek", Value = "uzb" });
            ////languageItems.Add(new SelectListItem { Text = "Uzbek - Cyrilic", Value = "uzb_cyrl" });
            //languageItems.Add(new SelectListItem { Text = "Vietnamese", Value = "vie" });
            //languageItems.Add(new SelectListItem { Text = "Yiddish", Value = "yid" });

        }

        public void ImmediateCleanup(string imageFilePath, string imageFileExtension, string textFilePath) {
            try {
                System.IO.File.Delete(imageFilePath);
                System.IO.File.Delete(textFilePath + ".txt");
                if (imageFileExtension.ToLower() == ".pdf") {
                    System.IO.File.Delete(textFilePath + ".tif");
                }
            }
            catch (Exception ex) {
                _debugLogger.Debug(ex, "Failed to delete OCR files.");
                // HANDLE ERROR
            }
        }

        public void Cleanup(string imageFilePath, string textFilePath) {
            // cleanup old files that are 2 hours or more old
            // this is a problem because we're doing it during the call from client
            // so find a way to thread it

            DirectoryInfo di = new DirectoryInfo(imageFilePath);
            foreach(var file in di.GetFiles("*.*")) {
                if (file.CreationTime.AddHours(2) < DateTime.Now) {
                    try {
                        file.Delete();
                    }
                    catch (Exception ex) {
                        _debugLogger.Debug(ex, $"Couldn't delete file {file.FullName}");
                    }
                }
            }

            di = new DirectoryInfo(textFilePath);
            foreach (var file in di.GetFiles("*.*")) {
                if (file.CreationTime.AddHours(2) < DateTime.Now) {
                    try {
                        file.Delete();
                    }
                    catch (Exception ex) {
                        _debugLogger.Debug(ex, $"Couldn't delete file {file.FullName}");
                    }
                }
            }
        }
    }
}
