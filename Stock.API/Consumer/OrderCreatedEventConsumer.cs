using MassTransit;
using MassTransit.RabbitMqTransport.Topology;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Stock.API.Services;

namespace Stock.API.Consumer
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        readonly MongoDbService _mongoDbService;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(MongoDbService mongoDbService, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _mongoDbService = mongoDbService;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            IMongoCollection<Models.Stock> collection = _mongoDbService.GetCollection<Models.Stock>();

            foreach (var orderItem in context.Message.OrderItems)
            {
                stockResult.Add(await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId.ToString() &&
                s.Count > orderItem.Count)).AnyAsync());
            }

            if (stockResult.TrueForAll(s => s.Equals(true))) 
            {
                //Stock güncellemesi
                foreach (var item in context.Message.OrderItems)
                {
                  var stock = await (await collection.FindAsync(s => s.ProductId == item.ProductId.ToString())).FirstOrDefaultAsync();
                    stock.Count -= item.Count;

                    await collection.FindOneAndReplaceAsync(x => x.ProductId == item.ProductId.ToString(), stock);

                }
                 var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));
                //Paymenti uyaracak event
                StockReserveEvent stockReserveEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    TotalPrice = context.Message.TotalPrice,
                    OrderItems = context.Message.OrderItems
                };
                await sendEndpoint.Send(stockReserveEvent);
            }
            else 
            {
                StockNotReserveEvent stockNotReserveEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    Message = "Stok Miktarı yetersiz"
                };
                await _publishEndpoint.Publish(stockNotReserveEvent);
                //Stok işlemi başarısız
                //Order için event
            }   
        }
    }
}
