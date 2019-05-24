using System.IO;

namespace DocumentRender.DocumentConverter
{
    internal class WindowsDocumentConverter : IDocumentConverter
    {
        public byte[] ConvertPage(string fileName, int page)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return RenderFirstPageInBytes(stream);
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
    }
}
