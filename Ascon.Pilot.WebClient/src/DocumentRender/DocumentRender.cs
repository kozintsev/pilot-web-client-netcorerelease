using System;
using DocumentRender.DocumentConverter;

namespace DocumentRender
{
    public class DocumentRender : IDocumentRender, IDisposable
    {
        private readonly IDocumentConverterFactory _converterFactory;

        public DocumentRender(IDocumentConverterFactory converterFactory)
        {
            _converterFactory = converterFactory;
        }

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

        //public Bitmap RenderFirstPage(string fileName)
        //{

        //    using (var stream = File.OpenRead(fileName))
        //    {
        //        return RenderFirstPage(stream);
        //    }
        //}

        public byte[] RenderFirstPage(string fileName)
        {
            var converter = _converterFactory.GetDocumentConverter();
            return converter.ConvertPage(fileName, 1);
        }

        //private Bitmap RenderFirstPage(Stream xpsStream)
        //{
        //    Bitmap result = null;
        //    using (var tileManager = new TilesManager(xpsStream))
        //    {
        //        for (var i = 0; i < tileManager.PageCount; i++)
        //        {
        //            int pageWidth = 0, pageHeight = 0;
        //            tileManager.LoadPage(i, ref pageWidth, ref pageHeight, false);
        //            var tile = new Tile
        //            {
        //                PageNum = i,
        //                Height = pageHeight,
        //                Width = pageWidth,
        //                Scale = 0.5
        //            };
        //            result = tileManager.GetPage(tile, false);
        //        }
        //    }
        //    return result;
        //}
    }
}