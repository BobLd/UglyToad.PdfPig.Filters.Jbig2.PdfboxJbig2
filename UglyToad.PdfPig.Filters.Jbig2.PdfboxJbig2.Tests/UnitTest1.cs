namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Tests
{
    public class UnitTest1
    {
        private readonly ParsingOptions parsingOption = new ParsingOptions()
        {
            UseLenientParsing = true,
            SkipMissingFonts = true,
            FilterProvider = Jbig2DecodeFilterTests.MyFilterProvider.Instance
        };

        [Fact]
        public void Test1()
        {
            string pdfPath = Path.Combine("Documents", "new.pdf");

            using (var document = PdfDocument.Open(pdfPath, parsingOption))
            {
                var page = document.GetPage(1);
                int i = 0;
                foreach (var image in page.GetImages())
                {
                    if (image.TryGetPng(out var png))
                    {
                        File.WriteAllBytes($"new_{i++}.png", png);
                    }
                }
            }
        }
    }
}