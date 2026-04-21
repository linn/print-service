using Linn.Common.Messaging.RabbitMQ;
using Linn.PrintService.IoC;
using Linn.PrintService.Messaging.Host;
using Linn.PrintService.Messaging.Host.Handlers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServices();
builder.Services.AddLog();

builder.Services.AddSingleton<RabbitChannelConfiguration>(sp =>
    {
        var config = new RabbitChannelConfiguration(
            queueName: "print",               
            routingKeys: new[] { "print.job", "print.rsn.document", "print.packing-list.document", "print.invoice.document" },
            exchangeName: "print",
            durableExchange: true,
            createConsumerChannel: true,
            createProducerChannel: false
        );

        Console.WriteLine($"[Program] Rabbit config created: Queue={config.QueueName}");
        return config;
    });

builder.Services.AddSingleton<IMessageHandler, PrintJobMessageHandler>();
builder.Services.AddSingleton<IMessageHandler, PrintRsnDocumentMessageHandler>();
builder.Services.AddSingleton<IMessageHandler, PrintPackingListMessageHandler>();
builder.Services.AddSingleton<IMessageHandler, PrintInvoiceMessageHandler>();

builder.Services.AddHostedService<RabbitChannelInitializer>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

Console.WriteLine("[Program] Host built. Starting...");
host.Run();