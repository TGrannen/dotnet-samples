namespace QuestPdf.Console.Tests;

public class GraphicsShowcaseDocumentTests
{
    [Test]
    public Task Graphics_showcase_document_matches_snapshot()
    {
        var document = new GraphicsShowcaseDocument();
        return Verify(document);
    }
}
