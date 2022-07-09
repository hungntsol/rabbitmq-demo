using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQ.Service.Bus.Abstractions;

public interface IMessageBuilder
{
	IServiceCollection Services { get; }
}