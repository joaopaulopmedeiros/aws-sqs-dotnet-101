namespace Order.Core.Messaging;

public interface IEventHandler<TEvent>
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}