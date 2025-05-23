namespace UglyToad.PdfPig.Filters.Jbig2.PdfboxJbig2.Jbig2
{
    using System;

    /// <summary>
    /// Can be used if the maximum value limit of an integer is exceeded.
    /// </summary>
    internal sealed class IntegerMaxValueException : Jbig2Exception
    {
        public IntegerMaxValueException()
        {
        }

        public IntegerMaxValueException(string message)
            : base(message)
        {
        }

        public IntegerMaxValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
