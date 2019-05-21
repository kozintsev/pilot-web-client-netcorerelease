using System;
using System.Collections.Generic;
using System.Text;

namespace MuPDF
{
    public interface IDocumentRender
    {
        byte[] RenderFirstPage(string fileName);
    }
}
