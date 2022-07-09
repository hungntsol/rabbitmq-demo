namespace RabbitMQ.Service.Bus.Events;

public class OrderEvent : IntegrationEventBase
{
	public int Number { get; set; }
	public string Email { get; set; } = null!;
	public double Amount { get; set; }
}