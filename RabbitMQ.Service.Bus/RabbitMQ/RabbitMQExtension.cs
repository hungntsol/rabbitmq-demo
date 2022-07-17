using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Service.Bus.Abstractions;
using RabbitMQ.Service.Bus.Events;
using RabbitMQ.Service.Bus.RabbitMQ.Persistence;

namespace RabbitMQ.Service.Bus.RabbitMQ;

public static class RabbitMQExtension
{
	public static IMessageBuilder AddRabbitMQ(
		this IServiceCollection services,
		Action<MessageManagerSettings> messageManagerConfiguration,
		Action<QueueSettings> queueConfiguration)
	{
		var msgSettings = new MessageManagerSettings();
		messageManagerConfiguration.Invoke(msgSettings);
		services.AddSingleton(msgSettings);

		var queueSettings = new QueueSettings();
		queueConfiguration.Invoke(queueSettings);
		services.AddSingleton(queueSettings);
		
		
		services.AddSingleton<IRabbitMQPersistenceConnection, RabbitMQPersistenceConnection>();
		services.AddSingleton<IMessagePublisher, MessageManager>();

		return new DefaultMessageBuilder(services);
	}

	public static IMessageBuilder AddSubscriber<TObject, TSubscriber>(this IMessageBuilder builder)
		where TObject : IntegrationEventBase
		where TSubscriber : class, IMessageSubscriber<TObject>
	{
		builder.Services.AddHostedService<QueueListener<TObject>>();
		builder.Services.AddScoped<IMessageSubscriber<TObject>, TSubscriber>();

		return builder;
	}
}