using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Service.Bus.Abstractions;
using RabbitMQ.Service.Bus.Events;

namespace RabbitMQ.Service.Bus.RabbitMQ;

public class MessageManager : IMessagePublisher, IDisposable
{
	private const string MAX_PRIORITY_HEADER = "x-max-priority";

	internal IConnection Connection { get; private set; }
	internal IModel Channel { get; private set; }

	private readonly MessageManagerSettings _messageManagerSettings;
	private readonly QueueSettings _queueSettings;

	public MessageManager(MessageManagerSettings messageManagerSettings, QueueSettings queueSettings)
	{
		_messageManagerSettings = messageManagerSettings;
		_queueSettings = queueSettings;

		InitiateConnection();
		DeclareChannelExchange(messageManagerSettings);
		DeclareChannelQueue();
	}


	public Task PublishAsync<T>(T message, int priority = 1) where T : IntegrationEventBase
	{
		var sendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message,
			_messageManagerSettings.JsonSerializerOptions ?? JsonOptions.Default));

		var routingKey = _queueSettings.Queues.First(q => q.type == typeof(T)).name;
		return PublishMessageAsync(sendBytes.AsMemory(), routingKey, priority);
	}

	public IModel GetChannel()
	{
		return Channel;
	}

	public void MarkAsComplete(BasicDeliverEventArgs message) => Channel.BasicAck(message.DeliveryTag, false);
	public void MarkAsRejected(BasicDeliverEventArgs message) => Channel.BasicReject(message.DeliveryTag, false);
	
	public void Dispose()
	{
		if (Channel.IsOpen)
		{
			Channel.Close();
		}

		if (Connection.IsOpen)
		{
			Connection.Close();
		}
		
		GC.SuppressFinalize(this);
	}
	
	private void InitiateConnection()
	{
		var connectionFactory = new ConnectionFactory() { Uri = new Uri(_messageManagerSettings.HostAddress) };
		Connection = connectionFactory.CreateConnection();
		Channel = Connection.CreateModel();
	}

	private void DeclareChannelExchange(MessageManagerSettings messageManagerSettings)
	{
		if (messageManagerSettings.QueuePrefetch > 0)
		{
			Channel.BasicQos(0, _messageManagerSettings.QueuePrefetch, false);
		}

		Channel.ExchangeDeclare(
			_messageManagerSettings.ExchangeName,
			ExchangeType.Topic,
			true);
	}

	private void DeclareChannelQueue()
	{
		foreach (var queue in _queueSettings.Queues)
		{
			var args = new Dictionary<string, object>() { [MAX_PRIORITY_HEADER] = 10 };

			Channel.QueueDeclare(queue.name, true, false, false, args);
			Channel.QueueBind(queue.name, _messageManagerSettings.ExchangeName, queue.name, null);
		}
	}
	
	private Task PublishMessageAsync(ReadOnlyMemory<byte> body, string routingKey, int priority = 1)
	{
		var properties = Channel.CreateBasicProperties();
		properties.Persistent = false;
		properties.Priority = Convert.ToByte(priority);

		Channel.BasicPublish(_messageManagerSettings.ExchangeName, routingKey, properties, body);
		return Task.CompletedTask;
	}
}