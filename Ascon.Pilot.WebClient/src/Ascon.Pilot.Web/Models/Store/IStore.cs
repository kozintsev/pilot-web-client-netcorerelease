using System;
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

        string GetImagesStorageDirectory(Guid id);
    }

    class Store : IStore
    {
        private readonly object _lock = new object();
        private readonly string _tempDirectory;

        public Store()
        {
            _tempDirectory = Path.Combine(DirectoryProvider.CurrentDirectory, "storage");
        }

        public byte[] GetThumbnail(Guid id)
        {
            var imageFilename = GetThumbnailPath(id);
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
                    var filename = GetThumbnailPath(id);
                    Save(buffer, filename);
                }
            });
        }

        public byte[] GetImageFile(Guid id, int page)
        {
            var imageFilename = GetImagePath(id, page);
            if (!File.Exists(imageFilename))
                return null;

            using (var fileStream = File.OpenRead(imageFilename))
                return fileStream.ToByteArray();
        }

        public int GetFilePageCount(Guid fileId)
        {
            var imagesStoreDirectory = GetImagesStorageDirectory(fileId);
            if (!Directory.Exists(imagesStoreDirectory))
                return 0;

            var pages = Directory.EnumerateFiles(imagesStoreDirectory);
            return pages.Count();
        }

        public void PutImageFileAsync(Guid id, byte[] buffer, int page)
        {
            Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    var filename = GetImagePath(id, page);
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

        public string GetImagesStorageDirectory(Guid imageFileId)
        {
            var root = Path.Combine(_tempDirectory, "pages");
            var path = DirectoryProvider.GetStoragePath(imageFileId, root);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        private string GetThumbnailPath(Guid imageFileId)
        {
            var root = Path.Combine(_tempDirectory, "thumbs");
            var thumbnailPath = DirectoryProvider.GetStoragePath(imageFileId, root);
            var filename = Path.Combine(thumbnailPath, "thumb.png");
            return filename;
        }

        private string GetImagePath(Guid imageFileId, int page)
        {
            var imageDir = GetImagesStorageDirectory(imageFileId);
            return Path.Combine(imageDir, $"page_{page}.png");
        }
    }
}
