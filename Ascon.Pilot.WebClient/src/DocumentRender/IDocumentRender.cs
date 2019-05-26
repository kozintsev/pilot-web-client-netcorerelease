namespace DocumentRender
{
    public interface IDocumentRender
    {
        byte[] RenderPage(string fileName, int page);
    }
}
