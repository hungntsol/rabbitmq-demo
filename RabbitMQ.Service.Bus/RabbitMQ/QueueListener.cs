using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Service.Bus.Abstractions;
using RabbitMQ.Service.Bus.Events;

namespace RabbitMQ.Service.Bus.RabbitMQ;

public class QueueListener<T> : BackgroundService where T : IntegrationEventBase
{
	private readonly IMessagePublisher _messagePublisher;
	private readonly MessageManagerSettings _messageManagerSettings;
	private readonly ILogger<QueueListener<T>> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly string _queueName;

	public QueueListener(IMessagePublisher messagePublisher, MessageManagerSettings messageManagerSettings, ILogger<QueueListener<T>> logger, IServiceProvider serviceProvider, QueueSettings queueSettings)
	{
		_messagePublisher = messagePublisher;
		_messageManagerSettings = messageManagerSettings;
		_logger = logger;
		_serviceProvider = serviceProvider;

		_queueName = queueSettings.Queues.First(q => q.type == typeof(T)).name;
	}


	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		stoppingToken.ThrowIfCancellationRequested();

		var consumer = new EventingBasicConsumer(_messagePublisher.GetChannel());
		consumer.Received += async (sender, args) =>
		{
			try
			{
				var request = Encoding.UTF8.GetString(args.Body.Span);
				_logger.LogInformation("Message received: {Request}", request);

				using var scope = _serviceProvider.CreateScope();

				var receiver = scope.ServiceProvider.GetRequiredService<IMessageSubscriber<T>>();
				var response = JsonSerializer.Deserialize<T>(request,
					_messageManagerSettings.JsonSerializerOptions ?? JsonOptions.Default);

				if (response is not null)
				{
					await receiver.SubscribeAsync(response, stoppingToken);
				}

				_messagePublisher.MarkAsComplete(args);

				_logger.LogInformation("Message processed: {Request}", request);
			}
			catch (Exception e)
			{
				_messagePublisher.MarkAsRejected(args);
				_logger.LogError(e, "Error processing message");
			}

			stoppingToken.ThrowIfCancellationRequested();
		};

		_messagePublisher.GetChannel().BasicConsume(_queueName, false, consumer);

		return Task.CompletedTask;
	}

	public override Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("RabbitMQ listener started for {QueueName}", _queueName);

		return base.StartAsync(cancellationToken);
	}

	public override Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("RabbitMQ listener stopped for {QueueName}", _queueName);
		
		return base.StopAsync(cancellationToken);
	}
}