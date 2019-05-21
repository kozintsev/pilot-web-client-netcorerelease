using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MuPDF.DocumentConverter
{
    internal class LinuxDocumentConverter : IDocumentConverter
    {
        private readonly string _mutoolPath;

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
                    FileName = _mutoolPath,
                    Arguments = $"draw -o \"{resultPath}\" {fileName} 1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            //process.StandardOutput.ReadToEnd();
            process.WaitForExit();
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
            return Path.Combine(directory, "external", "mutool");
        }
    }
}
