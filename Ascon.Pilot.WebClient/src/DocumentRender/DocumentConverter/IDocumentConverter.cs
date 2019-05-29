namespace DocumentRender.DocumentConverter
{
    public interface IDocumentConverter
    {
        byte[] ConvertPage(string fileName, int page);
        byte[] ConvertPage(byte[] content, int page);
    }
}
