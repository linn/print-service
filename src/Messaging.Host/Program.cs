
using Linn.Common.Messaging.RabbitMQ;
using Linn.PrintService.Messaging.Host;
using Linn.PrintService.Messaging.Host.Handlers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<IMessageHandler, DebugHandler>();

var host = builder.Build();
host.Run();
