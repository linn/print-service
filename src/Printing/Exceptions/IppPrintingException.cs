namespace Linn.PrintService.Printing.Exceptions
{
    using System;

    public class IppPrintingException : Exception
    {
        public IppPrintingException(string message)
            : base(message)
        {
        }

        public IppPrintingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}