using MassTransit;
using Shared;
using Stock.API.Consumer;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(conf =>
{
    conf.AddConsumer<OrderCreatedEventConsumer>();
    conf.AddConsumer<PaymentFailedEventConsumer>();
    conf.UsingRabbitMq((context, _conf) =>
    {
        _conf.Host("localhost");
        _conf.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue,
            e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context)); 
        
        _conf.ReceiveEndpoint(RabbitMQSettings.Stock_PaymentFailedEventQueue,
            e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));

    });
});
builder.Services.AddSingleton<MongoDbService>();

var app = builder.Build();


app.Run();
