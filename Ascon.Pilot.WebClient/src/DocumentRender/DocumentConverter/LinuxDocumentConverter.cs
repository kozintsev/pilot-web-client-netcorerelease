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

        public byte[] ConvertPage(string fileName, int page)
        {
            var resultDir = Path.GetDirectoryName(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);
            var resultPath = Path.Combine(resultDir, $"{name}.png");

            Console.WriteLine(_mutoolPath);

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    //FileName = "/bin/bash",
                    //Arguments = $"-c \"{_mutoolPath} -o {resultPath} {fileName}\"",
                    FileName = _mutoolPath,
                    Arguments = $"-o {resultPath} {fileName}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            _logger.InfoFormat(result);
            return FileToBytes(resultPath);
        }

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
