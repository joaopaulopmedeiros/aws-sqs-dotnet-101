using Order.Core.Events;
using Order.Core.Messaging;

namespace Order.Worker.Handlers;

public sealed class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    : IEventHandler<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Processing order {OrderId} for customer {CustomerId}",
            @event.OrderId, @event.CustomerId);

        await Task.Delay(50, cancellationToken);

        logger.LogInformation("Order {OrderId} processed", @event.OrderId);
    }
}