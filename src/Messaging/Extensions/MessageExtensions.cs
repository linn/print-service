namespace Linn.PrintService.Messaging.Extensions
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

        public static bool TryGetHeaderAsBool(this Message message, string key, out bool value)
        {
            if (message.Headers.TryGetValue(key, out var obj))
            {
                var str = Encoding.UTF8.GetString((byte[])obj);
                return bool.TryParse(str, out value);
            }

            value = false;
            return false;
        }
    }
}
