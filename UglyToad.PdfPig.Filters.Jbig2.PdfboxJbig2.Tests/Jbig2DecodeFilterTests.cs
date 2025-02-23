using UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Tests.Images;
using UglyToad.PdfPig.Tokens;

namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Tests
{
    public class Jbig2DecodeFilterTests
    {
        private static readonly Lazy<string> DocumentFolder = new Lazy<string>(() => Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Documents")));

        [Fact]
        public void CanDecodeJbig2CompressedImageData_WithoutGlobalSegments()
        {
            var encodedImageBytes = ImageHelpers.LoadFileBytes("sampledata_page1.jb2");

            var filter = new PdfboxJbig2DecodeFilter();
            var dictionary = new Dictionary<NameToken, IToken>()
            {
                { NameToken.Filter, NameToken.Jbig2Decode }
            };

            var expectedBytes = ImageHelpers.LoadFileBytes("sampledata_page1.jb2-decoded.bin");
            var decodedBytes = filter.Decode(encodedImageBytes, new DictionaryToken(dictionary), TestFilterProvider.Instance, 0);

            Assert.True(expectedBytes.AsSpan().SequenceEqual(decodedBytes.Span));
        }

        [Fact]
        public void CanDecodeJbig2CompressedImageData_WithGlobalSegments()
        {
            var encodedGlobalsBytes = ImageHelpers.LoadFileBytes("globals.jb2");
            var encodedImageBytes = ImageHelpers.LoadFileBytes("img-refs-globals.jb2");

            var filter = new PdfboxJbig2DecodeFilter();
            var dictionary = new Dictionary<NameToken, IToken>
            {
                { NameToken.Filter, NameToken.Jbig2Decode },
                { NameToken.DecodeParms, new DictionaryToken(new Dictionary<NameToken, IToken>
                    {
                        { NameToken.Jbig2Globals, new StreamToken(new DictionaryToken(new Dictionary<NameToken, IToken>()), encodedGlobalsBytes) }
                    })
                },
                { NameToken.ImageMask, BooleanToken.True }
            };

            var expectedBytes = ImageHelpers.LoadFileBytes("img-refs-globals-decoded.bin", isCompressed: true);
            var decodedBytes = filter.Decode(encodedImageBytes, new DictionaryToken(dictionary), TestFilterProvider.Instance, 0);

            Assert.True(expectedBytes.AsSpan().SequenceEqual(decodedBytes.Span));
        }

        private readonly ParsingOptions parsingOption = new ParsingOptions()
        {
            UseLenientParsing = true,
            SkipMissingFonts = true,
            FilterProvider = MyFilterProvider.Instance
        };

        [Theory]
        [MemberData(nameof(GetAllDocuments))]
        public void CanExtractJbigImages(string documentName)
        {
            Directory.CreateDirectory("images");

            string docName = documentName.Substring(0, documentName.Length - 4);

            // Add the full path back on, we removed it so we could see it in the test explorer.
            documentName = Path.Combine(DocumentFolder.Value, documentName);

            bool hasRenderedJbig2 = false;
            using (var document = PdfDocument.Open(documentName, parsingOption))
            {
                for (var p = 0; p < document.NumberOfPages; p++)
                {
                    var page = document.GetPage(p + 1);

                    int i = 0;
                    foreach (var image in page.GetImages())
                    {
                        if (image.ImageDictionary.TryGet(NameToken.Filter, out NameToken filter) &&
                            filter.Data.Equals(NameToken.Jbig2Decode.Data))
                        {
                            Assert.True(image.TryGetPng(out var png));

                            File.WriteAllBytes(Path.Combine("images", $"{docName}_{p}_{i++}.png"), png);
                            hasRenderedJbig2 = true;
                        }
                        else if (image.ImageDictionary.TryGet(NameToken.F, out NameToken filter2) &&
                                 filter2.Data.Equals(NameToken.Jbig2Decode.Data))
                        {
                            Assert.True(image.TryGetPng(out var png));

                            File.WriteAllBytes(Path.Combine("images", $"{docName}_{p}_{i++}.png"), png);
                            hasRenderedJbig2 = true;
                        }
                    }

                    if (hasRenderedJbig2)
                    {
                        return;
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetAllDocuments
        {
            get
            {
                var files = Directory.GetFiles(DocumentFolder.Value, "*.pdf");

                // Return the shortname so we can see it in the test explorer.
                return files.Select(x => new object[] { Path.GetFileName(x) });
            }
        }

        public sealed class MyFilterProvider : BaseFilterProvider
        {
            /// <summary>
            /// The single instance of this provider.
            /// </summary>
            public static readonly IFilterProvider Instance = new MyFilterProvider();

            /// <inheritdoc/>
            private MyFilterProvider() : base(GetDictionary())
            {
            }

            private static Dictionary<string, IFilter> GetDictionary()
            {
                var ascii85 = new Ascii85Filter();
                var asciiHex = new AsciiHexDecodeFilter();
                var ccitt = new CcittFaxDecodeFilter();
                var dct = new DctDecodeFilter();
                var flate = new FlateFilter();
                var jbig2 = new PdfboxJbig2DecodeFilter(); // new filter
                var jpx = new JpxDecodeFilter();
                var runLength = new RunLengthFilter();
                var lzw = new LzwFilter();

                return new Dictionary<string, IFilter>
                {
                    { NameToken.Ascii85Decode.Data, ascii85 },
                    { NameToken.Ascii85DecodeAbbreviation.Data, ascii85 },
                    { NameToken.AsciiHexDecode.Data, asciiHex },
                    { NameToken.AsciiHexDecodeAbbreviation.Data, asciiHex },
                    { NameToken.CcittfaxDecode.Data, ccitt },
                    { NameToken.CcittfaxDecodeAbbreviation.Data, ccitt },
                    { NameToken.DctDecode.Data, dct },
                    { NameToken.DctDecodeAbbreviation.Data, dct },
                    { NameToken.FlateDecode.Data, flate },
                    { NameToken.FlateDecodeAbbreviation.Data, flate },
                    { NameToken.Jbig2Decode.Data, jbig2 }, // new filter
                    { NameToken.JpxDecode.Data, jpx },
                    { NameToken.RunLengthDecode.Data, runLength },
                    { NameToken.RunLengthDecodeAbbreviation.Data, runLength },
                    { NameToken.LzwDecode.Data, lzw },
                    { NameToken.LzwDecodeAbbreviation.Data, lzw }
                };
            }
        }

    }
}
