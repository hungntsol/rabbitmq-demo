using RabbitMQ.Service.Bus.Abstractions;
using RabbitMQ.Service.Bus.Events;

namespace RabbitMQ.Consumer.Api.MessageSubscriber;

public class OrderMessageSubscriber : IMessageSubscriber<OrderEvent>
{
	private readonly ILogger<OrderMessageSubscriber> _logger;
	private readonly IMessagePublisher _messagePublisher;

	public OrderMessageSubscriber(ILogger<OrderMessageSubscriber> logger, IMessagePublisher messagePublisher)
	{
		_logger = logger;
		_messagePublisher = messagePublisher;
	}

	public async Task SubscribeAsync(OrderEvent message, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Processing order {OrderNumber}", message.Number);

		await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
		
		_logger.LogInformation("End processing order {OrderNumber}", message.Number);

		await _messagePublisher.PublishAsync(new InvoiceEvent() { OrderNumber = message.Number });
	}
}