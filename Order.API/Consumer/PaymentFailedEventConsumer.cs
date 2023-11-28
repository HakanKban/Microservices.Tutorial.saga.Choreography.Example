using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.Context;
using Shared.Events;

namespace Order.API.Consumer
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly OrderAPIDbContext _context;

        public PaymentFailedEventConsumer(OrderAPIDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {

            var order = await _context.Orders.Where(x => x.Id == context.Message.OrderId).FirstOrDefaultAsync();

            order.OrderStatus = Models.OrderStatus.Fail;
            await _context.SaveChangesAsync();

        }
    }
}
