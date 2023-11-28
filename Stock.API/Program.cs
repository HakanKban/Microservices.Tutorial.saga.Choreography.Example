using MassTransit;
using Shared;
using Stock.API.Consumer;
using Stock.API.Services;
using MongoDB.Driver;

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
using IServiceScope scope = app.Services.CreateScope();
var mongoDBService = scope.ServiceProvider.GetService<MongoDbService>();
var collection = mongoDBService.GetCollection<Stock.API.Models.Stock>();
if(!collection.FindSync(session => true).Any()) 
{
    await collection.InsertOneAsync(new Stock.API.Models.Stock() { ProductId = Guid.NewGuid().ToString(), Count = 100 });
    await collection.InsertOneAsync(new Stock.API.Models.Stock() { ProductId = Guid.NewGuid().ToString(), Count = 200 });
    await collection.InsertOneAsync(new Stock.API.Models.Stock() { ProductId = Guid.NewGuid().ToString(), Count = 10 });
    await collection.InsertOneAsync(new Stock.API.Models.Stock() { ProductId = Guid.NewGuid().ToString(), Count = 25 });
    await collection.InsertOneAsync(new Stock.API.Models.Stock() { ProductId = Guid.NewGuid().ToString(), Count = 45 });
}

app.Run();
