using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ascon.Pilot.Transport;
using Ascon.Pilot.Web.Utils;
using DocumentRender;

namespace Ascon.Pilot.Web.Models.Store
{
    public interface IStore
    {
        byte[] GetImageFile(IRepository repository, Guid id, int size, string extension, int page);
    }

    class Store : IStore
    {
        private readonly IDocumentRender _render;

        public Store(IDocumentRender render)
        {
            _render = render;
        }

        public byte[] GetImageFile(IRepository repository, Guid id, int size, string extension, int page)
        {
            var png = $"page_{page}.png";
            var tempDirectory = DirectoryProvider.GetThumbnailsDirectory();

            var imageFilename = Path.Combine(tempDirectory, id.ToString(), png);
            if (File.Exists(imageFilename))
            {
                using (var fileStream = File.OpenRead(imageFilename))
                    return fileStream.ToByteArray();
            }

            var file = repository.GetFileChunk(id, 0, size);
            if (file == null)
                return null;

            if (!extension.Contains("xps"))
                return null;

            var fileName = $"{id}{extension}";
            var xpsFilename = Path.Combine(tempDirectory, fileName);
            using (var fileStream = File.Create(xpsFilename))
                fileStream.Write(file, 0, file.Length);

            var thumbnailContent = _render.RenderPage(xpsFilename, page);
            Save(thumbnailContent, imageFilename);
            return thumbnailContent;
        }

        private void Save(byte[] bytes, string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var fileStream = File.Create(filename))
                fileStream.Write(bytes, 0, bytes.Length);
        }
    }
}
