using System;
using System.IO;
using System.Reflection;

namespace DocumentRender
{
    static class DirectoryProvider
    {
        public static string GetCurrentDirectory()
        {
            var executeAssLocation = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(executeAssLocation);
            if (string.IsNullOrEmpty(directory))
                throw new InvalidOperationException();

            return directory;
        }

        public static string GetImageOutputDir(string fileName)
        {
            var resultDir = Path.GetDirectoryName(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);
            var outputDir = Path.Combine(resultDir, name);

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            return outputDir;
        }
    }
}
