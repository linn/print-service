namespace Linn.PrintService.Messaging.Exceptions
{
    using System;

    public class RsnPrintMessageException : Exception
    {
        public RsnPrintMessageException(string message)
            : base(message)
        {
        }

        public RsnPrintMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
