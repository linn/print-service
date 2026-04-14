namespace Linn.PrintService.Messaging.Host.Extensions
{
    using System.Text;

    using Linn.Common.Messaging.RabbitMQ;

    public static class MessageExtensions
    {
        public static bool TryGetHeaderAsString(this Message message, string key, out string value)
        {
            if (message.Headers.TryGetValue(key, out var obj))
            {
                value = Encoding.UTF8.GetString((byte[])obj);
                return true;
            }

            value = null;
            return false;
        }
    }
}
