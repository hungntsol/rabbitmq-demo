using RabbitMQ.Service.Bus.Abstractions;
using RabbitMQ.Service.Bus.Events;

namespace RabbitMQ.Consumer.Api.MessageSubscriber;

public class InvoiceMessageSubscriber : IMessageSubscriber<InvoiceEvent>
{
	private readonly ILogger<InvoiceMessageSubscriber> _logger;


	public InvoiceMessageSubscriber(ILogger<InvoiceMessageSubscriber> logger)
	{
		_logger = logger;
	}

	public async Task SubscribeAsync(InvoiceEvent message, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Creating invoice for {OrderNumber}", message.OrderNumber);

		await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
		
		_logger.LogInformation("End creating invoice for {OrderNumber}", message.OrderNumber);
	}
}