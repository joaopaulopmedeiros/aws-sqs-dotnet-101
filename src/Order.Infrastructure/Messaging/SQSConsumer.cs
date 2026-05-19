using System.Runtime.CompilerServices;
using System.Text.Json;

using Amazon.SQS;
using Amazon.SQS.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Order.Core.Messaging;

namespace Order.Infrastructure.Messaging;

public sealed class SQSConsumer<TEvent>(
    IAmazonSQS sqsClient,
    IServiceScopeFactory scopeFactory,
    SQSConsumerOptions _options,
    ILogger<SQSConsumer<TEvent>> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogConsumerIsSubscribing(_options.Queue);

        await Parallel.ForEachAsync(
            ReceiveMessagesAsync(cancellationToken),
            new ParallelOptions
            {
                MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism,
                CancellationToken = cancellationToken
            },
            async (message, ct) => await ProcessMessageAsync(message, ct));
    }

    private async IAsyncEnumerable<Message> ReceiveMessagesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ReceiveMessageRequest request = new()
        {
            QueueUrl = _options.Queue,
            MaxNumberOfMessages = _options.MaxNumberOfMessages,
            WaitTimeSeconds = _options.WaitTimeSeconds,
            MessageAttributeNames = ["All"],
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            ReceiveMessageResponse response = await sqsClient.ReceiveMessageAsync(request, cancellationToken);
            foreach (Message message in response.Messages ?? [])
                yield return message;
        }
    }

    private async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
    {
        try
        {
            TEvent @event = JsonSerializer.Deserialize<TEvent>(message.Body)!;

            await using var scope = scopeFactory.CreateAsyncScope();
            IEventHandler<TEvent> handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();

            await handler.HandleAsync(@event, cancellationToken);

            await sqsClient.DeleteMessageAsync(_options.Queue, message.ReceiptHandle, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process message {MessageId}", message.MessageId);
        }
    }
}