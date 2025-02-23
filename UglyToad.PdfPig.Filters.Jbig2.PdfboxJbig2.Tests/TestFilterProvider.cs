using UglyToad.PdfPig.Tokenization.Scanner;
using UglyToad.PdfPig.Tokens;

namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Tests
{
    internal class TestFilterProvider : ILookupFilterProvider
    {
        public static readonly TestFilterProvider Instance = new TestFilterProvider();

        public IReadOnlyList<IFilter> GetFilters(DictionaryToken dictionary)
        {
            return new List<IFilter>();
        }

        public IReadOnlyList<IFilter> GetNamedFilters(IReadOnlyList<NameToken> names)
        {
            return new List<IFilter>();
        }

        public IReadOnlyList<IFilter> GetAllFilters()
        {
            return new List<IFilter>();
        }

        public IReadOnlyList<IFilter> GetFilters(DictionaryToken dictionary, IPdfTokenScanner scanner)
        {
            return new List<IFilter>();
        }
    }
}
