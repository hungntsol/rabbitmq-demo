using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Service.Bus.Events;

namespace RabbitMQ.Service.Bus.Abstractions;

public interface IMessagePublisher
{
	Task PublishAsync<T>(T message, int priority = 1) where T : IntegrationEventBase;

	IModel GetChannel();

	void MarkAsComplete(BasicDeliverEventArgs message);
	void MarkAsRejected(BasicDeliverEventArgs message);
}