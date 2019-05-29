using System;
using System.IO;
using System.Threading.Tasks;
using Ascon.Pilot.Transport;
using Ascon.Pilot.Web.Utils;

namespace Ascon.Pilot.Web.Models.Store
{
    public interface IStore
    {
        byte[] GetImageFile(Guid id, int page);
        void PutImageFileAsync(Guid id, byte[] buffer, int page);
    }

    class Store : IStore
    {
        private readonly object _lock = new object();

        public byte[] GetImageFile(Guid id, int page)
        {
            var png = $"page_{page}.png";
            var tempDirectory = GetStorageDirectory();

            var imageFilename = Path.Combine(tempDirectory, id.ToString(), png);
            if (File.Exists(imageFilename))
            {
                using (var fileStream = File.OpenRead(imageFilename))
                    return fileStream.ToByteArray();
            }

            return null;
        }

        public void PutImageFileAsync(Guid id, byte[] buffer, int page)
        {
            Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    var png = $"page_{page}.png";
                    var storeDirectory = GetStorageDirectory();
                    var imageDir = Path.Combine(storeDirectory, id.ToString());
                    if (!Directory.Exists(imageDir))
                        Directory.CreateDirectory(imageDir);

                    var filename = Path.Combine(imageDir, png);
                    Save(buffer, filename);
                }
            });
        }

        private void Save(byte[] bytes, string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var fileStream = File.Create(filename))
                fileStream.Write(bytes, 0, bytes.Length);
        }

        private string GetStorageDirectory()
        {
            var directory = Path.Combine(DirectoryProvider.CurrentDirectory, "storage");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }
    }
}
