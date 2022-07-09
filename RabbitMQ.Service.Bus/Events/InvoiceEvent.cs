namespace RabbitMQ.Service.Bus.Events;

public class InvoiceEvent : IntegrationEventBase
{
	public int OrderNumber { get; set; }
}