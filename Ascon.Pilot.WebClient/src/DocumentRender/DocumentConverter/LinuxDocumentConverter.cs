using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using log4net;

namespace DocumentRender.DocumentConverter
{
    internal class LinuxDocumentConverter : IDocumentConverter
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LinuxDocumentConverter));
        private readonly DrawToolProvider _toolProvider;

        public LinuxDocumentConverter()
        {
            _toolProvider = new DrawToolProvider(_logger);
        }

        public byte[] ConvertPage(string fileName, int page)
        {
            var tool = _toolProvider.GetDrawTool();
            var arguments = tool.GetArguments(fileName, page, out var drawResultPath);
            RunDrawProcess(tool.ToolName, arguments);

            var bytes = FileToBytes(drawResultPath);
            DeleteFileSave(drawResultPath);
            return bytes;
        }

        public byte[] ConvertPage(byte[] content, int page)
        {
            var filename = SaveFile(content);
            var result = ConvertPage(filename, page);
            DeleteFileSave(filename);
            return result;
        }

        public IEnumerable<byte[]> ConvertFile(byte[] content)
        {
            throw new NotImplementedException();
        }

        private void RunDrawProcess(string toolName, string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = toolName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            _logger.InfoFormat(result);

            process.WaitForExit();
        }

        private string SaveFile(byte[] content)
        {
            var tempDirectory = DirectoryProvider.GetCurrentTempDirectory();
            var xpsFilename = Path.Combine(tempDirectory, Guid.NewGuid() + ".xps");
            using (var fileStream = File.Create(xpsFilename))
                fileStream.Write(content, 0, content.Length);

            return xpsFilename;
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

        private void DeleteFileSave(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch
            {
                // ignored
            }
        }
    }
}
