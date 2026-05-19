using System.Text.Json;

using Amazon.SQS;
using Amazon.SQS.Model;

using Order.Core.Messaging;

namespace Order.Infrastructure.Messaging;

public sealed class SQSProducer<TEvent>(IAmazonSQS sqsClient, SQSProducerOptions options) : IProducer<TEvent>
{
    public async Task ProduceAsync(TEvent @event, CancellationToken cancellationToken)
    {
        string body = JsonSerializer.Serialize(@event);

        await sqsClient.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = options.Queue,
            MessageBody = body
        }, cancellationToken);
    }
}