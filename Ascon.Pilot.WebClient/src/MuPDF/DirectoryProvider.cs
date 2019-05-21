using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MuPDF
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
    }
}
