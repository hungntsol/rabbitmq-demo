using RabbitMQ.Service.Bus.Events;

namespace RabbitMQ.Service.Bus.Abstractions;

public interface IMessageSubscriber<in T> where T : IntegrationEventBase
{
	Task SubscribeAsync(T message, CancellationToken cancellationToken);
}