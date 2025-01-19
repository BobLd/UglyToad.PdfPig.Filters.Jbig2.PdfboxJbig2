namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Jbig2
{
    /// <summary>
    /// Base class for all nodes in a Huffman tree.
    /// </summary>
    internal abstract class Node
    {
        public abstract long Decode(IImageInputStream iis);
    }
}