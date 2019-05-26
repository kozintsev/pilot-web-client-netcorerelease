using System.IO;

namespace DocumentRender.DocumentConverter
{
    internal class WindowsDocumentConverter : IDocumentConverter
    {
        public byte[] ConvertPage(string fileName, int page)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var bytes = RenderFirstPageInBytes(stream);
                var outputDir = DirectoryProvider.GetImageOutputDir(fileName);
                var resultPath = Path.Combine(outputDir, $"page_{page}.png");
                Save(bytes, resultPath);
                return bytes;
            }
        }

        private byte[] RenderFirstPageInBytes(Stream xpsStream)
        {
            byte[] result = null;
            using (var tileManager = new TilesManager(xpsStream))
            {
                for (var i = 0; i < tileManager.PageCount; i++)
                {
                    int pageWidth = 0, pageHeight = 0;
                    tileManager.LoadPage(i, ref pageWidth, ref pageHeight, false);
                    var scale = 0.5;
                    var tile = new Tile
                    {
                        PageNum = i,
                        Height = (int)(pageHeight * scale),
                        Width = (int)(pageWidth * scale),
                        Scale = scale
                    };
                    result = tileManager.GetPageInBytes(tile, false);
                }
            }
            return result;
        }

        private void Save(byte[] bytes, string filename)
        {
            using (var fileStream = File.Create(filename))
                fileStream.Write(bytes, 0, bytes.Length);
        }
    }
}
