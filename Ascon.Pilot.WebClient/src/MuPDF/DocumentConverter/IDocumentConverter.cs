using System;
using System.Collections.Generic;
using System.Text;

namespace MuPDF.DocumentConverter
{
    public interface IDocumentConverter
    {
        byte[] ConvertPage(string fileName, int page);
    }
}
