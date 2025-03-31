using static Application.Model.Orders.Order;

namespace Application.ViewModel.Orders
{
    public class OrderStateViewModel
    {
        public bool IsEnough { get; set; } 
        public States State { get; set; }
    }
}
