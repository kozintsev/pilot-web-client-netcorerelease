using System;
using System.IO;
using System.Drawing;

namespace MuPDF
{
    public class MuPdf : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
        }


        protected virtual void Dispose(bool bDisposing)
        {
            if (bDisposing)
            {
                // No need to call the finalizer since we've now cleaned
                // up the unmanaged memory
                GC.SuppressFinalize(this);
            }
        }

        public Bitmap RenderFirstPage(string fileName)
        {

            using (var stream = File.OpenRead(fileName))
            {
                return RenderFirstPage(stream);
            }
        }

        public byte[] RenderFirstPageInBytes(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return RenderFirstPageInBytes(stream);
            }
        }

        private Bitmap RenderFirstPage(Stream xpsStream)
        {
            Bitmap result = null;
            using (var tileManager = new TilesManager(xpsStream))
            {
                for (var i = 0; i < tileManager.PageCount; i++)
                {
                    int pageWidth = 0, pageHeight = 0;
                    tileManager.LoadPage(i, ref pageWidth, ref pageHeight, false);
                    var tile = new Tile
                    {
                        PageNum = i,
                        Height = pageHeight,
                        Width = pageWidth,
                        Scale = 0.5
                    };
                    result = tileManager.GetPage(tile, false);
                }
            }
            return result;
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
                        Height = (int) (pageHeight * scale),
                        Width = (int) (pageWidth * scale),
                        Scale = scale
                    };
                    result = tileManager.GetPageInBytes(tile, false);
                }
            }
            return result;
        }
    }
}