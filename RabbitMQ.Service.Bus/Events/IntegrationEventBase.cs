namespace RabbitMQ.Service.Bus.Events;

public class IntegrationEventBase
{
	public Guid Id { get; }
	public DateTime CreatedAt { get;  }

	public IntegrationEventBase()
	{
		Id = Guid.NewGuid();
		CreatedAt = DateTime.UtcNow;
	}
	
	public IntegrationEventBase(Guid id, DateTime createdAt)
	{
		Id = id;
		CreatedAt = createdAt;
	}
}