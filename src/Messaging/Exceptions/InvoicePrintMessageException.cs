namespace Linn.PrintService.Messaging.Exceptions
{
    using System;

    public class InvoicePrintMessageException : Exception
    {
        public InvoicePrintMessageException(string message)
            : base(message)
        {
        }

        public InvoicePrintMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
