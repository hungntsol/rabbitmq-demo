using RabbitMQ.Consumer.Api.MessageSubscriber;
using RabbitMQ.Service.Bus.Events;
using RabbitMQ.Service.Bus.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);


ConfigureServices(builder.Services, builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
	services.AddRabbitMQ(settings =>
			{
				settings.HostAddress = configuration.GetValue<string>("RabbitMQ:HostAddress");
				settings.ExchangeName = configuration.GetValue<string>("RabbitMQ:ExchangeName");
				settings.QueuePrefetch = configuration.GetValue<ushort>("RabbitMQ:QueuePrefetch");
			},
			queues =>
			{
				queues.AddQueue<OrderEvent>();
				queues.AddQueue<InvoiceEvent>();
			})
		.AddSubscriber<OrderEvent, OrderMessageSubscriber>()
		.AddSubscriber<InvoiceEvent, InvoiceMessageSubscriber>();
}