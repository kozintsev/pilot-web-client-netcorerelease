using System.Collections.Generic;

namespace DocumentRender
{
    public interface IDocumentRender
    {
        byte[] RenderPage(byte[] content, int page);
        IEnumerable<byte[]> RenderPages(byte[] content);
    }
}
