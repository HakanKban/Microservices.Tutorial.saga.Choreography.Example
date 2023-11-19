namespace Order.API.ViewModel
{
    public class CreateOrderVM
    {
        public string BuyerId { get; set; }
        public List<CreateOrderItemVM> CreateOrderItemVMs { get; set; }
    }
}
