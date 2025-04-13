using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages
{
    public class OrderDoneModel : PageModel
    {
        private readonly DataManager dataManager;

        public OrderDoneModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }


        public void OnGet(int orderId)
        {
        }
    }
}