namespace Order.Infrastructure.Messaging;

public sealed class SQSConsumerOptions
{
    public string Queue { get; set; } = string.Empty;
    public int MaxNumberOfMessages { get; set; } = 10;
    public int WaitTimeSeconds { get; set; } = 5;
    public int MaxDegreeOfParallelism { get; set; } = 4;
}