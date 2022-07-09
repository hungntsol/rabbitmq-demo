using System.Text.Json;
using RabbitMQ.Service.Bus.Abstractions;

namespace RabbitMQ.Service.Bus.RabbitMQ;

public class MessageManagerSettings
{
	public string HostAddress { get; set; } = null!;
	public string ExchangeName { get; set; } = null!;
	// public string ExchangeType { get; set; } = Client.ExchangeType.Direct;
	public ushort QueuePrefetch { get; set; }
	public JsonSerializerOptions JsonSerializerOptions { get; set; } = JsonOptions.Default;
}

