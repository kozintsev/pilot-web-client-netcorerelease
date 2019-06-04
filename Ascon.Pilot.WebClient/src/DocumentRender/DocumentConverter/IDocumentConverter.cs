using System.Collections.Generic;

namespace DocumentRender.DocumentConverter
{
    public interface IDocumentConverter
    {
        byte[] ConvertPage(byte[] content, int page);
        //IEnumerable<byte[]> ConvertFile(byte[] content);
        int ConvertFileToFolder(byte[] content, string rootFolder);
    }
}
