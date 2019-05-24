namespace DocumentRender.DocumentConverter
{
    public interface IDocumentConverter
    {
        byte[] ConvertPage(string fileName, int page);
    }
}
