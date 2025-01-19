namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Tests.Images
{
    using System.IO;
    using System.IO.Compression;

    // From PdfPig

    public static class ImageHelpers
    {
        private static readonly string FilesFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Images", "Files"));

        public static byte[] LoadFileBytes(string filename, bool isCompressed = false)
        {
            var filePath = Path.Combine(FilesFolder, filename);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            var memoryStream = new MemoryStream();
            if (isCompressed)
            {
                using (var deflateStream = new DeflateStream(File.OpenRead(filePath), CompressionMode.Decompress))
                {
                    deflateStream.CopyTo(memoryStream);
                }

                return memoryStream.ToArray();
            }

            return File.ReadAllBytes(filePath);
        }
    }
}
