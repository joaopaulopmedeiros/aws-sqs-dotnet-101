var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSQSConsumer<OrderCreatedEvent, OrderCreatedEventHandler>("Messaging:QueueUrl");

await builder.Build().RunAsync();