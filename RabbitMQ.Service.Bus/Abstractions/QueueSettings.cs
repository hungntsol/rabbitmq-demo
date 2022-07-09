namespace RabbitMQ.Service.Bus.Abstractions;

public class QueueSettings
{
	internal IList<(string name, Type type)> Queues { get; } =
		new List<(string name, Type type)>();

	public void AddQueue<T>(string? name = null) where T : class
	{
		var type = typeof(T);
		// var routing = routingKey ?? name;
		Queues.Add((name ?? type.FullName, type)!);
	}
}