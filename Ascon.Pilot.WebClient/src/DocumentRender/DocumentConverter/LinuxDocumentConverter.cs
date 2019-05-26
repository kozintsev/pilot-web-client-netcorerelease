using System;
using System.Diagnostics;
using System.IO;
using log4net;

namespace DocumentRender.DocumentConverter
{
    internal class LinuxDocumentConverter : IDocumentConverter
    {
        private readonly string _mutoolPath;
        private readonly ILog _logger = LogManager.GetLogger(typeof(LinuxDocumentConverter));

        public LinuxDocumentConverter()
        {
            var directory = DirectoryProvider.GetCurrentDirectory();
            _mutoolPath = GetMutoolPath(directory);
        }

        //public bool IsConverted(string filename, int page)
        //{
        //    var outputDir = GetOutputDir(filename);
        //    var resultPath = Path.Combine(outputDir, $"page_{page}.png");
        //    return File.Exists(resultPath);
        //}

        public byte[] ConvertPage(string fileName, int page)
        {
            var outputDir = DirectoryProvider.GetImageOutputDir(fileName);
            var drawResultPath = Path.Combine(outputDir, "page_%d.png");
            
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _mutoolPath,
                    Arguments = $"-o {drawResultPath} {fileName}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            _logger.InfoFormat(result);
            var resultPath = Path.Combine(outputDir, $"page_{page}.png");
            _logger.Debug(resultPath);
            return FileToBytes(resultPath);
        }

        //public byte[] GetConvertedPage(string filename, int page)
        //{
        //    var outputDir = GetOutputDir(filename);
        //    var imageFilename = Path.Combine(outputDir, $"page_{page}.png");

        //    if (File.Exists(imageFilename))
        //    {
        //        using (var fileStream = File.OpenRead(imageFilename))
        //            return fileStream.ToByteArray();
        //    }

        //    return null;
        //}

        private static byte[] FileToBytes(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private string GetMutoolPath(string directory)
        {
            return Path.Combine(directory, "external", "mudraw");
        }
    }
}
