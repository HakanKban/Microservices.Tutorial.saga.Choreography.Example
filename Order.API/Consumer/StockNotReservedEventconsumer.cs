using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.Context;
using Shared.Events;

namespace Order.API.Consumer
{
    public class StockNotReservedEventconsumer : IConsumer<StockNotReserveEvent>
    {
        readonly OrderAPIDbContext _context;

        public StockNotReservedEventconsumer(OrderAPIDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<StockNotReserveEvent> context)
        {
            var order = await _context.Orders.Where(x => x.Id == context.Message.OrderId).FirstOrDefaultAsync();

            order.OrderStatus = Models.OrderStatus.Fail;
            await _context.SaveChangesAsync();
        }
    }
}
