using MassTransit;
using Shared.Events;
using Stock.API.Services;
using MongoDB.Driver;

namespace Stock.API.Consumer
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        readonly MongoDbService _mongoDbService;

        public PaymentFailedEventConsumer(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var stocks = _mongoDbService.GetCollection<Models.Stock>();

            foreach (var item in context.Message.OrderItemMessages)
            {

                var stock = await (await stocks.FindAsync(x => x.ProductId == item.ProductId)).FirstOrDefaultAsync();
                stock.Count += item.Count;
                await stocks.FindOneAndReplaceAsync(s => s.ProductId == item.ProductId, stock);
            }


            throw new NotImplementedException();
        }
    }
}
