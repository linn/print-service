namespace Linn.PrintService.Messaging.Extensions
{
    using System.Text;
    using System.Text.Json;

    using Linn.Common.Messaging.RabbitMQ;

    public static class MessageExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static T? DeserializeBody<T>(this Message message) where T : class
        {
            var json = Encoding.UTF8.GetString(message.Body.ToArray());
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
    }
}
