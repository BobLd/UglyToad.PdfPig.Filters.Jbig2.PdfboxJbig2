namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Tests
{
    using UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Jbig2;
    using UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Tests.Images;
    using Xunit;

    public class MMRDecompressorTest
    {
        [Fact]
        public void MmrDecodingTest()
        {
            var expected = new byte[]
            {
                0, 0, 2, 34, 38, 102, 239, 255, 2, 102, 102,
                238, 238, 239, 255, 255, 0, 2, 102, 102, 127,
                255, 255, 255, 0, 0, 0, 4, 68, 102, 102, 127
            };

            var iis = new ImageInputStream(ImageHelpers.LoadFileBytes("sampledata.jb2").AsMemory());
            // Sixth Segment (number 5)
            var sis = new SubInputStream(iis, 252, 38);
            var mmrd = new MMRDecompressor(16 * 4, 4, sis);
            Jbig2Bitmap b = mmrd.Uncompress();
            byte[] actual = b.ByteArray;

            Assert.Equal(expected, actual);
        }
    }
}
