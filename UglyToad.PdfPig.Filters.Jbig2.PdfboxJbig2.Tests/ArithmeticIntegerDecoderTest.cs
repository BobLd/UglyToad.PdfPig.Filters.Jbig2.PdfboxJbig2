namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Tests
{
    using UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Jbig2;
    using UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Tests.Images;
    using Xunit;

    public class ArithmeticIntegerDecoderTest
    {
        [Fact]
        public void DecodeTest()
        {
            var iis = new ImageInputStream(ImageHelpers.LoadFileBytes("arith-encoded-testsequence.bin"));
            var ad = new ArithmeticDecoder(iis);
            var aid = new ArithmeticIntegerDecoder(ad);

            long result = aid.Decode(null);

            Assert.Equal(1, result);
        }
    }
}
