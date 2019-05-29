namespace DocumentRender
{
    public interface IDocumentRender
    {
        byte[] RenderPage(byte[] content, int page);
    }
}
