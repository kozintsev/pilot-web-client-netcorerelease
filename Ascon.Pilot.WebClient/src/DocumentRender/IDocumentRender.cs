using System.Collections.Generic;

namespace DocumentRender
{
    public interface IDocumentRender
    {
        byte[] RenderPage(byte[] content, int page);
        int RenderPages(byte[] content, string rootFolder);
    }
}
