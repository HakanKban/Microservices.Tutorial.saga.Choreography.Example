using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Stock.API.Services;

namespace Stock.API.Consumer
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        readonly MongoDbService _mongoDbService;

        public OrderCreatedEventConsumer(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            IMongoCollection<Models.Stock> collection = _mongoDbService.GetCollection<Models.Stock>();

            foreach (var orderItem in context.Message.OrderItems)
            {
                stockResult.Add(await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId &&
                s.Count > orderItem.Count)).AnyAsync());
            }

            if (stockResult.TrueForAll(s => s.Equals(true))) 
            {
                //Stock güncellemesi
                foreach (var item in context.Message.OrderItems)
                {
                  var stock = await (await collection.FindAsync(s => s.ProductId == item.ProductId)).FirstOrDefaultAsync();
                    stock.Count -= item.Count;

                    await collection.FindOneAndReplaceAsync(x => x.ProductId == item.ProductId, stock);

                }
                //Paymenti uyaracak event
            
            
            
            }
            else 
            {
                //Stok işlemi başarısız
                //Order için event
                
            }   


        }
    }
}
