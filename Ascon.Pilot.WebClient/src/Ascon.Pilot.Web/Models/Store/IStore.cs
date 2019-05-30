using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ascon.Pilot.Transport;
using Ascon.Pilot.Web.Utils;

namespace Ascon.Pilot.Web.Models.Store
{
    public interface IStore
    {
        byte[] GetThumbnail(Guid id);
        void PutThumbnailAsync(Guid id, byte[] buffer);

        byte[] GetImageFile(Guid id, int page);

        int GetFilePageCount(Guid fileId);

        void PutImageFileAsync(Guid id, byte[] buffer, int page);
    }

    class Store : IStore
    {
        private readonly object _lock = new object();
        private readonly string _tempDirectory;

        public Store()
        {
            _tempDirectory = GetStorageDirectory();
        }

        public byte[] GetThumbnail(Guid id)
        {
            var png = "image.png";

            var imageFilename = Path.Combine(_tempDirectory, id + "_thumb", png);
            if (File.Exists(imageFilename))
            {
                using (var fileStream = File.OpenRead(imageFilename))
                    return fileStream.ToByteArray();
            }

            return null;
        }

        public void PutThumbnailAsync(Guid id, byte[] buffer)
        {
            Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    var png = "image.png";
                    var storeDirectory = GetStorageDirectory();
                    var imageDir = Path.Combine(storeDirectory, id + "_thumb");
                    if (!Directory.Exists(imageDir))
                        Directory.CreateDirectory(imageDir);

                    var filename = Path.Combine(imageDir, png);
                    Save(buffer, filename);
                }
            });
        }

        public byte[] GetImageFile(Guid id, int page)
        {
            var png = $"page_{page}.png";
            
            var imageFilename = Path.Combine(_tempDirectory, id.ToString(), png);
            if (File.Exists(imageFilename))
            {
                using (var fileStream = File.OpenRead(imageFilename))
                    return fileStream.ToByteArray();
            }

            return null;
        }

        public int GetFilePageCount(Guid fileId)
        {
            var imagesDirectory = Path.Combine(_tempDirectory, fileId.ToString());
            if (!Directory.Exists(imagesDirectory))
                return 0;

            var pages = Directory.EnumerateFiles(imagesDirectory);
            return pages.Count();
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
