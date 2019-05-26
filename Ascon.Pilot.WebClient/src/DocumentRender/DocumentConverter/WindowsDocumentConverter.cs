using System.IO;

namespace DocumentRender.DocumentConverter
{
    internal class WindowsDocumentConverter : IDocumentConverter
    {
        public byte[] ConvertPage(string fileName, int page)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return RenderFirstPageInBytes(stream, page);
            }
        }

        private byte[] RenderFirstPageInBytes(Stream xpsStream, int page)
        {
            using (var tileManager = new TilesManager(xpsStream))
            {
                return LoadPage(tileManager, page);
            }
        }

        private static byte[] LoadPage(TilesManager tileManager, int page)
        {
            var internalPage = page;
            if (page > 0)
                internalPage = page - 1;

            int pageWidth = 0, pageHeight = 0;
            tileManager.LoadPage(internalPage, ref pageWidth, ref pageHeight, false);
            var scale = 0.5;
            var tile = new Tile
            {
                PageNum = internalPage,
                Height = (int) (pageHeight * scale),
                Width = (int) (pageWidth * scale),
                Scale = scale
            };
            var result = tileManager.GetPageInBytes(tile, false);
            return result;
        }
    }
}
