namespace Linn.PrintService.Messaging.Host.Handlers
{
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Printing.Exceptions;
    using Linn.PrintService.Printing.Services;

    public class PrintJobMessageHandler : IMessageHandler
    {
        private readonly IIppPrintingService printingService;

        public PrintJobMessageHandler(IIppPrintingService printingService)
        {
            this.printingService = printingService;
        }

        public string RoutingKey { get; } = "print.job";

        public Task HandleAsync(Message message, CancellationToken cancellationToken)
        {
            return this.ProcessPrintJobAsync(message, cancellationToken);
        }

        private async Task ProcessPrintJobAsync(Message message, CancellationToken cancellationToken)
        {
            Console.WriteLine("[Handler] Received a message");

            try
            {
                if (!message.Headers.TryGetValue("printerUri", out var printerUriObj) ||
                    !message.Headers.TryGetValue("jobName", out var jobNameObj))
                {
                    Console.WriteLine("[Handler] Missing printerUri or jobName header");
                    return;
                }

                var printerUri = printerUriObj?.ToString() ?? throw new Exception("printerUri empty");
                var jobName = jobNameObj?.ToString() ?? "PrintJob";
                var data = message.Body.ToArray();

                Console.WriteLine($"[Handler] Processing print job: {jobName} for {printerUri}, {data.Length} bytes");

                await this.printingService.Print(printerUri, jobName, data);

                Console.WriteLine($"[Handler] Print job completed: {jobName}");
            }
            catch (IppPrintingException ex)
            {
                Console.WriteLine($"[Handler] IPP printing failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Handler] Handler exception: {ex}");
            }
        }
    }
}