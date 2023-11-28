using MassTransit;
using Payment.API.Consumers;
using Shared;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMassTransit(conf =>
{
    conf.AddConsumer<StokcReservedEventConsumer>();    
    conf.UsingRabbitMq((context, _conf) =>
    {
        _conf.Host("localhost");
        _conf.ReceiveEndpoint(RabbitMQSettings.Payment_StockReservedEventQueue,
           e => e.ConfigureConsumer<StokcReservedEventConsumer>(context));

    });
});

var app = builder.Build();


app.Run();
