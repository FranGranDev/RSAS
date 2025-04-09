using Application.Models;

namespace Application.ViewModel.Orders
{
    public class OrderStateViewModel
    {
        public bool IsEnough { get; set; }
        public Order.States State { get; set; }
    }
}