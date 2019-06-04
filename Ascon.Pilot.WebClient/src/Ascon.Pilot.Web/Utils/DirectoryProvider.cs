using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static string GetStoragePath(Guid fileId, string archiveRootFolder)
        {
            var bytes = fileId.ToByteArray();
            var result = Path.Combine(archiveRootFolder, bytes[14].ToString(CultureInfo.InvariantCulture), bytes[15].ToString(CultureInfo.InvariantCulture), fileId.ToString());
            return result;
        }

        public static string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;
    }
}
