using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.Context;
using Order.API.ViewModel;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(conf => 
{
    conf.UsingRabbitMq((context, _conf) =>
    {
        _conf.Host("localhost");
        
    });
});
builder.Services.AddDbContext<OrderAPIDbContext>(conf => conf.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapPost("/create-order", async (CreateOrderVM model, OrderAPIDbContext context, IPublishEndpoint publishEndpoint ) => 
{
    Order.API.Models.Order order = new()
    {
        BuyerId = Guid.TryParse(model.BuyerId, out Guid _buyerId) ? _buyerId : Guid.NewGuid(),
        OrderItems = model.CreateOrderItemVMs.Select(oi => new Order.API.Models.OrderItem()
        {
            Count = oi.Count,
            Price = oi.Price,
            ProductId = Guid.Parse(oi.ProductId)
        }).ToList(),
        OrderStatus = Order.API.Models.OrderStatus.Suspend,
        CreatedDate = DateTime.UtcNow,
        TotalPrice = model.CreateOrderItemVMs.Sum(oi => oi.Price* oi.Count),
    }; 
    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    OrderCreatedEvent orderCreatedEvent = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        TotalPrice = order.TotalPrice,
        OrderItems = order.OrderItems.Select(x => new Shared.Messages.OrderItemMessage() 
        { 
            Count = x.Count,
            Price = x.Price,
            ProductId = x.ProductId,
        }).ToList()
    };
    await publishEndpoint.Publish(orderCreatedEvent);
});

app.Run();
