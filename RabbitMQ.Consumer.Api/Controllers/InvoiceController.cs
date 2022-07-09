using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Service.Bus.Abstractions;
using RabbitMQ.Service.Bus.Events;

namespace RabbitMQ.Consumer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
	private readonly IMessagePublisher _messagePublisher;

	public InvoiceController(IMessagePublisher messagePublisher)
	{
		_messagePublisher = messagePublisher;
	}
	
	[HttpPost]
	public async Task<IActionResult> PostInvoice(InvoiceEvent message)
	{
		await _messagePublisher.PublishAsync(message);
		return Ok();
	}
}