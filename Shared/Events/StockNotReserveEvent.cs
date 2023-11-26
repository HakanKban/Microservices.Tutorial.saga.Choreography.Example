namespace Shared.Events;
public class StockNotReserveEvent
{
    public Guid OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public string Message { get; set; }
}
