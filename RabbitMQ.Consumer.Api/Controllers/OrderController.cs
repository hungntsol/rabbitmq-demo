using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Service.Bus.Abstractions;
using RabbitMQ.Service.Bus.Events;

namespace RabbitMQ.Consumer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
	private readonly IMessagePublisher _messagePublisher;

	public OrderController(IMessagePublisher messagePublisher)
	{
		_messagePublisher = messagePublisher;
	}

	[HttpPost]
	public async Task<IActionResult> PostOrder(OrderEvent orderEvent)
	{
		await _messagePublisher.PublishAsync(orderEvent);
		return Accepted();
	}
}