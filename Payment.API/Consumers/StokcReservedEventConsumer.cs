using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StokcReservedEventConsumer : IConsumer<StockReserveEvent>
    {
        readonly IPublishEndpoint _publishEndPoint;

        public StokcReservedEventConsumer(IPublishEndpoint publishEndPoint)
        {
            _publishEndPoint = publishEndPoint;
        }

        public async Task Consume(ConsumeContext<StockReserveEvent> context)
        {
            if(true) 
            {
                PaymentCompletedEvent paymentCompletedEvent = new PaymentCompletedEvent()
                {
                    OrderId = context.Message.OrderId
                };
                await _publishEndPoint.Publish(paymentCompletedEvent);
            }
            else 
            {
                PaymentFailedEvent paymentFailedEvent = new PaymentFailedEvent()
                {
                    OrderId = context.Message.OrderId,
                    Message = "Bakiye Yetersiz",
                    OrderItemMessages = context.Message.OrderItems
                };
                await _publishEndPoint.Publish(paymentFailedEvent);
            }
        }
    }
}
