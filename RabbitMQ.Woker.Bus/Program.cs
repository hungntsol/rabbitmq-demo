using RabbitMQ.Service.Bus.Events;
using RabbitMQ.Service.Bus.RabbitMQ;
using RabbitMQ.Woker.Bus.MessageSubscriber;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder, builder.Services);

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

void ConfigureServices(WebApplicationBuilder hostBuilderContext, IServiceCollection services)
{
	var configuration = hostBuilderContext.Configuration;

	services.AddRabbitMQ(settings =>
		{
			var section = configuration.GetSection("RabbitMQ");
			settings.HostAddress = section.GetValue<string>("HostAddress");
			settings.ExchangeName = section.GetValue<string>("ExchangeName");
			settings.QueuePrefetch = section.GetValue<ushort>("QueuePrefetch");
			settings.RetryCount = section.GetValue<int>("RetryCount");
		}, queues =>
		{
			queues.AddQueue<OrderEvent>();
			queues.AddQueue<InvoiceEvent>();
		})
		.AddSubscriber<OrderEvent, OrderMessageSubscriber>()
		.AddSubscriber<InvoiceEvent, InvoiceMessageSubscriber>();
}