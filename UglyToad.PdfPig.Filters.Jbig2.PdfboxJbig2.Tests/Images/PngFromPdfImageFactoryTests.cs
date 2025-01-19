namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Tests.Images
{
    using UglyToad.PdfPig.Graphics.Colors;

    public class PngFromPdfImageFactoryTests
    {
        /* TODO
        [Fact]
        public void CanGeneratePngFromJbig2DecodedImageData()
        {
            var decodedBytes = ImageHelpers.LoadFileBytes("sampledata_page1.jb2-decoded.bin");
            var image = new TestPdfImage
            {
                ColorSpaceDetails = DeviceGrayColorSpaceDetails.Instance,
                DecodedBytes = decodedBytes,
                WidthInSamples = 64,
                HeightInSamples = 56,
                BitsPerComponent = 1
            };

            Assert.True(PngFromPdfImageFactory.TryGenerate(image, out var bytes));
            Assert.True(ImageHelpers.ImagesAreEqual(LoadImage("sampledata_page1.jb2.png"), bytes));
        }
        */
    }
}
