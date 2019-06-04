using System;
using System.Collections.Generic;
using System.IO;
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

        public byte[] RenderPage(byte[] content, int page)
        {
            if (content == null)
                return null;

            var converter = _converterFactory.GetDocumentConverter();
            var converted = converter.ConvertPage(content, page);
            return converted;
        }

        public int RenderPages(byte[] content, string rootFolder)
        {
            if (content == null)
                return 0;

            var converter = _converterFactory.GetDocumentConverter();
            var converted = converter.ConvertFileToFolder(content, rootFolder);
            return converted;
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
    }
}