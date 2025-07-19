using UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Jbig2;
using UglyToad.PdfPig.Tokens;

namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2
{
    // based on https://github.com/UglyToad/PdfPig/pull/631, https://github.com/apache/pdfbox-jbig2

    /// <summary>
    /// JBIG2 Filter for monochrome image data.
    /// </summary>
    public sealed class PdfboxJbig2DecodeFilter : IFilter
    {
        /// <inheritdoc />
        public bool IsSupported => true;

        /// <inheritdoc />
        public Memory<byte> Decode(Memory<byte> input, DictionaryToken streamDictionary,
            IFilterProvider filterProvider, int filterIndex)
        {
            var decodeParms = DecodeParameterResolver.GetFilterParameters(streamDictionary, filterIndex);
            Jbig2Document? globalDocument = null;
            if (decodeParms.TryGet(NameToken.Jbig2Globals, out StreamToken tok) && !tok.Data.IsEmpty)
            {
                globalDocument = new Jbig2Document(new ImageInputStream(tok.Decode(filterProvider)));
            }

            using (var jbig2 = new Jbig2Document(new ImageInputStream(input), globalDocument?.GlobalSegments))
            {
                var page = jbig2.GetPage(1);
                var bitmap = page.GetBitmap();

                globalDocument?.Dispose();

                var data = bitmap.ByteArray;

                // We always invert bits - this makes test 'CanDecodeJbig2CompressedImageData_WithGlobalSegments' fail
                for (int i = 0; i < data.Length; ++i)
                {
                    ref byte x = ref data[i];
                    x = (byte)~x;
                }

                return data;
            }

            /*
            var pageInfo = (PageInformation)page.GetPageInformationSegment().GetSegmentData();
            // Invert bits if the default pixel value is black
            if (pageInfo.DefaultPixelValue == 0)
            {
                return bitmap.ByteArray;
            }
            */
        }

        /*
        private static bool IsImageMask(DictionaryToken streamDictionary)
        {
            if (streamDictionary.TryGet(NameToken.Mask, out ArrayToken _))
            {
                return true;
            }

            if (streamDictionary.TryGet(NameToken.ImageMask, out BooleanToken isImageMask))
            {
                return isImageMask.Data;
            }

            if (streamDictionary.TryGet(NameToken.Im, out BooleanToken isIm))
            {
                return isIm.Data;
            }

            return false;
        }
        */
    }
}
