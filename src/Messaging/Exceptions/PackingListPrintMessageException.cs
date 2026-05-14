namespace Linn.PrintService.Messaging.Exceptions
{
    using System;

    public class PackingListPrintMessageException : Exception
    {
        public PackingListPrintMessageException(string message)
            : base(message)
        {
        }

        public PackingListPrintMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
