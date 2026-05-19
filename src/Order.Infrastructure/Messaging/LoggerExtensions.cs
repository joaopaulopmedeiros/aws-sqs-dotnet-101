using Microsoft.Extensions.Logging;

namespace Order.Infrastructure.Messaging;

internal static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "SQS consumer subscribing to queue: {Queue}")]
    internal static partial void LogConsumerIsSubscribing(this ILogger logger, string queue);
}