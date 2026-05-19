namespace Order.Infrastructure.Messaging;

public sealed class SQSConsumerOptions
{
    public string Queue { get; init; } = string.Empty;
    public int MaxNumberOfMessages { get; init; } = 10;
    public int WaitTimeSeconds { get; init; } = 5;
    public int MaxDegreeOfParallelism { get; init; } = 4;
}