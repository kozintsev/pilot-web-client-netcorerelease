using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ascon.Pilot.Web.Utils
{
    public class DirectoryProvider
    {
        public static string GetThumbnailsDirectory()
        {
            var directory = "tmp/";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        public static string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;
    }
}
