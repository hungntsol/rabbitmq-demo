using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQ.Service.Bus.Abstractions;

public class DefaultMessageBuilder : IMessageBuilder
{
	public IServiceCollection Services { get; }

	public DefaultMessageBuilder(IServiceCollection services)
	{
		Services = services;
	}
}