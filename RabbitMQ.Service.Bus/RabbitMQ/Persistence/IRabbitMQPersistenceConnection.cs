using RabbitMQ.Client;

namespace RabbitMQ.Service.Bus.RabbitMQ.Persistence;

public interface IRabbitMQPersistenceConnection : IDisposable
{
	bool IsConnected { get; }
	
	IConnection? Connection { get; }

	bool RetryConnect();
	
	IModel CreateChannel();
}