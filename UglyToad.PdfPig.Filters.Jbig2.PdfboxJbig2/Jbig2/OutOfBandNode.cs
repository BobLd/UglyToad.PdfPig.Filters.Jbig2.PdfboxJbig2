namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Jbig2
{
    using static HuffmanTable;

    /// <summary>
    /// Represents an out-of-band node in a Huffman tree.
    /// </summary>
    internal sealed class OutOfBandNode : Node
    {
        public OutOfBandNode(Code _)
        {
        }

        public override long Decode(IImageInputStream iis)
        {
            return long.MaxValue;
        }
    }
}
