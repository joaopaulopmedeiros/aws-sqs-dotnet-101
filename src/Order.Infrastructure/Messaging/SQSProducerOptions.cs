namespace Order.Infrastructure.Messaging;

public sealed class SQSProducerOptions
{
    public string Queue { get; set; } = string.Empty;
}