using System.Diagnostics;
using UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Jbig2;
using UglyToad.PdfPig.Tokens;
using UglyToad.PdfPig.Util;

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

        [Conditional("DEBUG")]
        private static void ValidateRefs(DictionaryToken dictionaryToken)
        {
            if (dictionaryToken.GetObjectOrDefault(NameToken.Jbig2Globals) is IndirectReferenceToken)
            {
                throw new Exception($"Got a IndirectReferenceToken for '{NameToken.Jbig2Globals}'.");
            }

            if (dictionaryToken.GetObjectOrDefault(NameToken.ImageMask) is IndirectReferenceToken)
            {
                throw new Exception($"Got a IndirectReferenceToken for '{NameToken.ImageMask}'.");
            }

            if (dictionaryToken.GetObjectOrDefault(NameToken.Im) is IndirectReferenceToken)
            {
                throw new Exception($"Got a IndirectReferenceToken for '{NameToken.Im}'.");
            }

            if (dictionaryToken.GetObjectOrDefault(NameToken.DecodeParms, NameToken.Dp) is IndirectReferenceToken)
            {
                throw new Exception($"Got a IndirectReferenceToken for '{NameToken.Jbig2Globals}'.");
            }

            if (dictionaryToken.GetObjectOrDefault(NameToken.Filter, NameToken.F) is IndirectReferenceToken)
            {
                throw new Exception($"Got a IndirectReferenceToken for '{NameToken.Jbig2Globals}'.");
            }
        }

        /// <inheritdoc />
        public ReadOnlyMemory<byte> Decode(ReadOnlySpan<byte> input, DictionaryToken streamDictionary, int filterIndex)
        {
            ValidateRefs(streamDictionary);

            var decodeParms = DecodeParameterResolver.GetFilterParameters(streamDictionary, filterIndex);
            Jbig2Document globalDocument = null;
            if (decodeParms.TryGet(NameToken.Jbig2Globals, out StreamToken tok))
            {
                globalDocument = new Jbig2Document(new ImageInputStream(tok.Data.ToArray()));
            }

            using (var jbig2 = new Jbig2Document(new ImageInputStream(input.ToArray()),
                       globalDocument != null ? globalDocument.GlobalSegments : null))
            {
                var page = jbig2.GetPage(1);
                var bitmap = page.GetBitmap();

                var pageInfo = (PageInformation)page.GetPageInformationSegment().GetSegmentData();

                globalDocument?.Dispose();

                if (pageInfo.DefaultPixelValue == 0 && !IsImageMask(streamDictionary))
                {
                    return bitmap.ByteArray;
                }

                var data = bitmap.ByteArray;

                // Invert bits if the default pixel value is black
                for (int i = 0; i < data.Length; ++i)
                {
                    ref byte x = ref data[i];
                    x = (byte)~x;
                }

                return data;
            }
        }

        private static bool IsImageMask(DictionaryToken streamDictionary)
        {
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
    }
}
