using Amazon.Runtime;
using Amazon.SQS;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using Order.Core.Messaging;
using Order.Infrastructure.Messaging;

namespace Order.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSQSProducer<TEvent>(
        this IServiceCollection services,
        string queueUrlConfigKey)
    {
        services.TryAddSingleton<IAmazonSQS>(sp =>
        {
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();

            AmazonSQSConfig config = new()
            {
                ServiceURL = configuration["AWS:ServiceURL"]
            };

            return new AmazonSQSClient(new BasicAWSCredentials("test", "test"), config);
        });

        services.AddScoped<IProducer<TEvent>>(sp =>
        {
            IAmazonSQS sqsClient = sp.GetRequiredService<IAmazonSQS>();
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
            return new SQSProducer<TEvent>(sqsClient, configuration[queueUrlConfigKey]!);
        });

        return services;
    }

    public static IServiceCollection AddSQSConsumer<TEvent, THandler>(
        this IServiceCollection services,
        string queueUrlConfigKey)
        where THandler : class, IEventHandler<TEvent>
    {
        services.TryAddSingleton<IAmazonSQS>(sp =>
        {
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();

            AmazonSQSConfig config = new()
            {
                ServiceURL = configuration["AWS:ServiceURL"]
            };

            return new AmazonSQSClient(new BasicAWSCredentials("test", "test"), config);
        });

        services.AddScoped<IEventHandler<TEvent>, THandler>();

        services.AddHostedService(sp => new SQSConsumer<TEvent>(
            sp.GetRequiredService<IAmazonSQS>(),
            sp.GetRequiredService<IServiceScopeFactory>(),
            sp.GetRequiredService<IConfiguration>()[queueUrlConfigKey]!,
            sp.GetRequiredService<ILogger<SQSConsumer<TEvent>>>()
        ));

        return services;
    }
}