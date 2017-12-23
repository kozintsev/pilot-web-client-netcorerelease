using System.IO.Compression;

namespace Ascon.Pilot.WebClient.Extensions
{
    public static class ZipArchiveExt
    {
        public static ZipArchiveDirectory CreateDirectory(this ZipArchive @this, string directoryPath)
        {
            return new ZipArchiveDirectory(@this, directoryPath);
        }
    }

    public class ZipArchiveDirectory
    {
        private readonly string _directory;

        internal ZipArchiveDirectory(ZipArchive archive, string directory)
        {
            Archive = archive;
            _directory = directory;
        }

        public ZipArchive Archive { get; }

        public ZipArchiveEntry CreateEntry(string entry)
        {
            return Archive.CreateEntry(_directory + "/" + entry);
        }

        public ZipArchiveEntry CreateEntry(string entry, CompressionLevel compressionLevel)
        {
            return Archive.CreateEntry(_directory + "/" + entry, compressionLevel);
        }
    }
}
